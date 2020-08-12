using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public const byte MAX_NAME_LENGTH = 30;

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
        /// <param name="errMsg"> Error message </param>
        /// <returns> 
        ///     Returns the status of the parse<br/> 
        ///     True - Success<br/>
        ///     False - Failure
        /// </returns>
        public static bool ParseRequestStr(string srcStr, string[] sParams, string[] rParams, out ParamInfo[] _params, out string errMsg)
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
                if (!ProcessSingleUseParams(srcStr, sParams, paramsInfo, out errMsg))
                    return false;

            // If rParams are provided, process them
            if (rParams != null)
                if (!ProcessRepeatableUseParams(srcStr, rParams, paramsInfo, out errMsg))
                    return false;
                
            // If text was passed but none matched our parameters:
            if (paramsInfo.Count == 0)
            {
                errMsg = "Invalid Request. The request must contain a valid parameter to be used in some way.";
                return false;
            }

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

            // We need to check the beginning of the srcStr for format errors 
            if (pOrdered[0].ParamPropertyStartIndex - pOrdered[0].ParamType.Length > 0)
            {                
                string fullErrStr = srcStr.Trim();

                string onlyErrStr = fullErrStr.Substring(0, pOrdered[0].ParamPropertyStartIndex - 1).Trim();

                errMsg = FormatError(onlyErrStr, "[" + onlyErrStr + "] " + fullErrStr.Substring(pOrdered[0].ParamPropertyStartIndex - 1));
                return false;
            }

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

        /// <summary>
        ///     Validates the name given follows the rules bluequery enforces.<br/>
        ///     @param - name, Proposted name of given entity<br/>
        ///     @param - errMsg, Error message
        ///     Returns the status of the validation<br/>
        ///     True - Success<br/>
        ///     False - Failure
        /// </summary>
        /// <param name="name"> Name given </param>
        /// <param name="errMsg"> Error message </param>
        /// <returns></returns>
        public static bool ValidateName(ref string name, out string errMsg)
        {
            name = name.Trim();

            // We trimmed it so no need to check for whitespace               
            if (string.IsNullOrEmpty(name))
            {
                errMsg = $"Invalid name given. The name '{name}' cannot be an empty string or only contain whitespaces.";
                return false;
            }
            else if (name.Length > MAX_NAME_LENGTH)
            {
                errMsg = $"Invalid name given. The name '{name}' exceeds the {MAX_NAME_LENGTH} character limit of names. Choose a smaller name.";
                return false;
            }
            else
            {
                errMsg = string.Empty;
                return true;
            }
        }
        
        private static bool ProcessSingleUseParams(in string srcStr, string[] sParams, List<ParamInfo> _paramsInfo, out string errMsg)
        {
            Dictionary<string, int> offsets = new Dictionary<string, int>();

            errMsg = string.Empty;            

            // We need this copy because the Contains() doesn't allow us to check if something is contained after a specific index within a string
            string srcStrCpy = srcStr;

            // Check for each repeatable parameter
            for (int i = 0; i < sParams.Length; i++)
            {                
                if (srcStrCpy.Contains(sParams[i]))
                {
                    int propIndex = srcStr.IndexOf(sParams[i], offsets.ContainsKey(sParams[i]) ? offsets[sParams[i]] : 0);
                    int valueIndex = propIndex + sParams[i].Length;

                    // If a single use param is already present in our collection then report error                        
                    if (offsets.ContainsKey(sParams[i]))
                    {
                        errMsg = $"Invalid use of single use parameter. The parameter {sParams[i].Trim()} cannot be used more than once.";
                        return false;
                    }

                    // Saving the offset of this param
                    offsets.Add(sParams[i], valueIndex);
                    // Adding the param and its information to our collection
                    _paramsInfo.Add(new ParamInfo
                    {
                        ParamType = sParams[i],
                        ParamPropertyStartIndex = propIndex
                    });
                        
                    // Updating the copy by removing the accounted for singe use param
                    srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(sParams[i]), sParams[i].Length);
                }                         
            }
            return true;
        }

        private static bool ProcessRepeatableUseParams(in string srcStr, string[] rParams, List<ParamInfo> _paramsInfo, out string errMsg)
        {
            Dictionary<string, int> offsets = new Dictionary<string, int>();
            errMsg = string.Empty;            

            // We need this copy because the Contains() doesn't allow us to check if something is contained after a specific index within a string
            string srcStrCpy = srcStr;

            for (int i = 0; i < rParams.Length; i++)
            {
                while(true)
                {
                    if (srcStrCpy.Contains(rParams[i]))
                    {
                        int propIndex = srcStr.IndexOf(rParams[i], offsets.ContainsKey(rParams[i]) ? offsets[rParams[i]] : 0);
                        int valueIndex = propIndex + rParams[i].Length;

                        if (offsets.ContainsKey(rParams[i]))
                        {
                            offsets[rParams[i]] = valueIndex;
                        }
                        else
                        {
                            // Saving the offset of this param
                            offsets.Add(rParams[i], valueIndex);
                        }

                        // Adding the param and its information to our collection
                        _paramsInfo.Add(new ParamInfo
                        {
                            ParamType = rParams[i],
                            ParamPropertyStartIndex = propIndex
                        });

                        // Updating the copy by removing the accounted for singe use param
                        srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(rParams[i]), rParams[i].Length);
                    }
                    else
                        break;
                }
            }

            // Updaing the copy
            //srcStrCpy = srcStrCpy.Remove(srcStrCpy.IndexOf(s_or_r_params[i]), s_or_r_params[i].Length);
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
                string fullErrStr = _srcStr;

                fullErrStr = fullErrStr.Insert(errParam.ParamValueStartIndex, "[");
                fullErrStr = fullErrStr.Insert(errParam.ParamValueStartIndex + errParam.ParamValue.Length + 1, "]");

                errMsg = FormatError(errParam.ParamValue, fullErrStr);
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Formats a formatting error to be returned to the user<br/>
        ///     @param - onlyErrStr, Isolated error string
        ///     @param - fullErrStr, Error in the entire context
        /// </summary>
        /// <param name="onlyErrStr"> Isolated error string </param>
        /// <param name="fullErrStr"> Error string with full context </param>
        /// <returns></returns>
        private static string FormatError(in string onlyErrStr, in string fullErrStr)
        {
            return "Formatting Error Detected.\n" + "**Error Text:**\n" + $"```css\n[{onlyErrStr}]``````fix\n{fullErrStr}```";
        }
    }
}