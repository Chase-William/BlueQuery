using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;

namespace BlueQuery.Util
{
    /// <summary>
    ///     Provides information needed to parse the given string containing a user's request.
    /// </summary>
    public struct ParamInfo
    {
        /// <summary>
        ///     Identifer of what the param purpose is
        /// </summary>
        public string ParamType { get; set; }

        /// <summary>
        ///     Start index of the property ex. -rename
        /// </summary>
        public int ParamPropertyStartIndex { get; set; }

        /// <summary>
        ///     Start index of the param value 
        /// </summary>
        public int ParamValueStartIndex => ParamPropertyStartIndex + ParamType.Length;

        /// <summary>
        ///     Value of the parameter
        /// </summary>
        public string ParamValue { get; set; }
    }    

    public class StrParseUtil
    {

        /// <summary>
        ///     Parses a request string.<br/>
        ///     @param - _srcStr, Request string to be parsed<br/>
        ///     @param - sParams, Single use parameters to be parsed, passing null for this param will result in no single use params being processed<br/>
        ///     @param - rParams, Repeatable parameters to be parsed, passing null for this param will result in no repeatable params being processed<br/>
        ///     @out param - Parsed parameters<br/>
        ///     Returns the status of the parse<br/> 
        ///     True - Success<br/>
        ///     False - Failure
        /// </summary>
        /// <param name="srcStr"> Request string to be parsed </param>
        /// <param name="sParams"> Single use parameters to be parsed </param>
        /// <param name="rParams"> Repeatable parameters to be parsed </param>
        /// <param name="_params"> Parsed parameters </param>
        /// <returns> 
        ///     Returns the status of the parse<br/> 
        ///     True - Success<br/>
        ///     False - Failure
        /// </returns>
        public static bool ParseRequestStr(in string srcStr, string[] sParams, string[] rParams, out ParamInfo[] _params, out string errMsg)
        {
            _params = null;

            // If the given string was empty return null
            if (string.IsNullOrWhiteSpace(srcStr))
            {
                errMsg = "Invalid Request. The request provided was empty.";
                return false;
            }

            // Collection that holds all parameter's info
            var paramsInfo = new List<ParamInfo>();

            // If sParams are provided, process them
            if (sParams != null)
                if (!ProcessParams(srcStr, sParams, paramsInfo, ProcessMode.Single, out errMsg))
                    return false;

            // If rParams are provided, process them
            if (rParams != null)
                if (!ProcessParams(srcStr, rParams, paramsInfo, ProcessMode.Repeatable, out errMsg))
                    return false;
                

            // Ordering all the parameters by their start index from small to large
            var pOrdered = paramsInfo.OrderBy(x => x.ParamValueStartIndex).ToArray();

            // Getting the parameters values now
            // Using the starting index of the next ordered param as the stopping point
            for (int i = 0; i < pOrdered.Length; i++)
            {
                // If we are iterating on the last item we need to read until the end because their isn't another element in the array (index out of bounds incoming)
                if (i == (pOrdered.Length - 1))
                {
                    pOrdered[i].ParamValue = srcStr.Substring(pOrdered[i].ParamValueStartIndex);
                    break;
                }
                // Reading from the start index of the current until the start index of the next minus 1
                // This is assigned back into the array but to the ParamValue property
                var test = (pOrdered[(i + 1)].ParamValueStartIndex - pOrdered[(i + 1)].ParamPropertyStartIndex);
                pOrdered[i].ParamValue = srcStr.Substring(pOrdered[i].ParamValueStartIndex, pOrdered[(i + 1)].ParamPropertyStartIndex - pOrdered[i].ParamValueStartIndex);
            }

            /* ----- Formatting Error Checks ----- */

            // If these return false then we want to propagate the error 

            if (sParams != null)            
                if (!CheckFormatting(srcStr, sParams, pOrdered, out errMsg))             
                    return false;

            if (rParams != null)
                if (!CheckFormatting(srcStr, rParams, pOrdered, out errMsg))
                    return false;

            // Updaing the out var with the ordered and filled collection
            _params = pOrdered;
            errMsg = string.Empty;
            return true;
        }

        enum ProcessMode
        {
            Single = 0,
            Repeatable
        }

        // Gets parameters that can be repeated throughout the request
        private static bool ProcessParams(in string srcStr, string[] s_or_r_params, List<ParamInfo> _paramsInfo, ProcessMode mode, out string errMsg)
        {
            errMsg = string.Empty;
            // Prevents IndexOf() call not find the same _param repeatedly 
            int posOffset = 0;

            // We need this copy because the Contains() doesn't allow us to check if something is contained after a specific index within a string
            string srcStrCpy = srcStr;

            // Check for each repeatable parameter
            for (int i = 0; i < s_or_r_params.Length; i++)
            {
                // Interate until all repeatable parameters captured
                while (true)
                {
                    if (srcStrCpy.Contains(s_or_r_params[i]))
                    {
                        int propIndex = srcStr.IndexOf(s_or_r_params[i], posOffset);
                        int valueIndex = propIndex + s_or_r_params[i].Length;

                        // Setting a offset so that the next iteration of IndexOf won't return the index of the same _param
                        posOffset = valueIndex;

                        _paramsInfo.Add(new ParamInfo
                        {
                            ParamType = s_or_r_params[i],
                            ParamPropertyStartIndex = propIndex
                        });

                        // If we are processing for single use parameters, propagate an error if a single use param shows up more than once
                        if (mode.Equals(ProcessMode.Single))
                        {
                            if (_paramsInfo.Where(p => p.ParamType.Equals(s_or_r_params[i])).Count() > 1)
                            {
                                errMsg = "Invalid use of single use parameter. A single use parameter can only be used once in a request.";
                                return false;
                            }
                        }

                        // Updaing the copy
                        srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(s_or_r_params[i]), s_or_r_params[i].Length);
                    }
                    else 
                        break;
                }                
            }
            return true;
        }

        /// <summary>
        ///     Checks to see if any of the parameters values contains any keywords which would indicate a formatting error.<br/>
        ///     returns:<br/>
        ///     True == Formatting passes<br/>
        ///     False == Formatting failure
        /// </summary>
        /// <param name="s_or_r_params"> Single or Repeatable parameters </param>
        /// <param name="_paramsInfo"> Ordered parameters </param>
        /// <param name="errMsg"> Error message </param>
        /// <returns></returns>
        private static bool CheckFormatting(in string _srcStr, in string[] s_or_r_params, ParamInfo[] _paramsInfo, out string errMsg)
        {
            errMsg = string.Empty;
            ParamInfo errParam = new ParamInfo();
            // Looking for any parameter values that contain a reserved keyword within them
            // This would most likely be a user input error
            if (s_or_r_params.Any(s_str => _paramsInfo.Any(p =>
            {
                if (p.ParamValue.Contains(s_str.Trim()))
                {
                    errParam = p;
                    return true;
                }
                else
                    return false;
            })))
            {
                string srcStrCpy = _srcStr;

                srcStrCpy = srcStrCpy.Insert(errParam.ParamValueStartIndex, "[");
                srcStrCpy = srcStrCpy.Insert(errParam.ParamValueStartIndex + errParam.ParamValue.Length + 1, "]");

                errMsg = "Formatting Error Detected.\n" + "**Error Text:**\n" + $"```css\n[{errParam.ParamValue}]``````fix\n{srcStrCpy}```";
                return false;
            }
            return true;
        }
    }
}