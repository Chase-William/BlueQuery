using System;
using System.Collections.Generic;
using System.Linq;
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

    public class ParseUtil
    {

        /// <summary>
        ///     Parses a request string.<br/>
        ///     @param - _srcStr, Request string to be parsed<br/>
        ///     @param - sParams, Single use parameters to be parsed, passing null for this param will result in no repeatable params being processed<br/>
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
                ProcessSingleUseParams(srcStr, sParams, paramsInfo);
                
            // If rParams are provided, process them
            if (rParams != null)
                ProcessRepeatableParams(srcStr, rParams, paramsInfo);
                

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

            // Looking for any parameter values that contain a reserved keyword within them
            // This would most likely be a user input error
            if (sParams.Any(s_str => pOrdered.Any(p => p.ParamValue.Contains(s_str.Trim()))) || rParams.Any(r_str => pOrdered.Any(p => p.ParamValue.Contains(r_str.Trim()))))
            {
                errMsg = "Parameter value contained a reserved keyword.";
                return false;
            }

            // Updaing the out var with the ordered and filled collection
            _params = pOrdered;
            errMsg = string.Empty;
            return true;
        }

        private static void ProcessSingleUseParams(in string srcStr, string[] sParams, List<ParamInfo> _params)
        {
            for (int i = 0; i < sParams.Length; i++)
            {
                if (srcStr.Contains(sParams[i]))                
                    _params.Add(new ParamInfo
                    {
                        ParamType = sParams[i],
                        ParamPropertyStartIndex = srcStr.IndexOf(sParams[i])
                    });                
            }
        }

        // Gets parameters that can be repeated throughout the request
        private static void ProcessRepeatableParams(in string srcStr, string[] rParams, List<ParamInfo> _params)
        {
            // Prevents IndexOf() call not find the same _param repeatedly 
            int posOffset = 0;

            // We need this copy because the Contains() doesn't allow us to check if something is contained after a specific index within a string
            string srcStrCpy = srcStr;

            // Check for each repeatable parameter
            for (int i = 0; i < rParams.Length; i++)
            {
                // Interate until all repeatable parameters captured
                while (true)
                {
                    if (srcStrCpy.Contains(rParams[i]))
                    {
                        int propIndex = srcStr.IndexOf(rParams[i], posOffset);
                        int valueIndex = propIndex + rParams[i].Length;

                        // Setting a offset so that the next iteration of IndexOf won't return the index of the same _param
                        posOffset = valueIndex;

                        _params.Add(new ParamInfo
                        {
                            ParamType = rParams[i],
                            ParamPropertyStartIndex = propIndex
                        });

                        // Updaing the copy
                        srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(rParams[i]), rParams[i].Length);
                    }
                    else 
                        break;
                }
            }
        }
    }
}