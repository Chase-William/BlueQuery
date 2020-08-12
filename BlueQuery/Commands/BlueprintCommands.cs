using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using BlueQuery.Util;
using System.Linq;
using BlueQueryLibrary.Data;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata.Ecma335;

namespace BlueQuery.Commands
{
    public class BlueprintCommands
    {
        const string NAME_PARAM = " -name ";
        const string CREATE_PARAM = " -create ";
        const string TRIBE_PARAM = " -tribe ";
        const string IMG_LINK_PARAM = " -img ";

        public readonly string[] SINGLE_USE_PARAMS = { NAME_PARAM, TRIBE_PARAM, CREATE_PARAM, IMG_LINK_PARAM };

        [Command("Blueprint")]
        [Description("Base command for performing CRUD operations on blueprints.")]
        [Aliases("blueprint", "bp")]
        public async Task OnSave(CommandContext _ctx)
        {            
            if(!StrParseUtil.ParseRequestStr(_ctx.RawArgumentString, SINGLE_USE_PARAMS, null, out ParamInfo[] @params, out string errMsg))
            {
                await _ctx.RespondAsync(errMsg);
                return;
            }

            /* ----- Command Specific Logic Below ----- */

            // If no arguments were given return
            if (@params == null)
            {
                await _ctx.RespondAsync("No parameters given. The blueprint command isn't useful by itself. Try passing some parameters to make it do something.");
                return;
            }

            // To create a blueprint right now you need to pass the -create param and the -tribe param
            if (@params.Any(p => p.ParamType.Equals(CREATE_PARAM) && @params.Any(p => p.ParamType.Equals(TRIBE_PARAM))))
            {
                string imgUrl = string.Empty;
                string bpNameOnly = string.Empty;
                string bpNameWithExtension = string.Empty;

                // Validate and process the given name
                bpNameOnly = (@params.Single(p => p.ParamType.Equals(CREATE_PARAM)).ParamValue);
                if (!StrParseUtil.ValidateName(ref bpNameOnly, out errMsg))
                {
                    await _ctx.RespondAsync(errMsg);
                    return;
                }
                bpNameWithExtension = bpNameOnly;

                // Check to make sure the given tribe exist
                if (!TribeDatabaseContext.Provider.DoesTribeExist(@params.Single(t => t.ParamType.Equals(TRIBE_PARAM)).ParamValue, out Tribe tribe, out errMsg))
                {
                    await _ctx.RespondAsync(errMsg);
                    return;
                }

                // Check to make sure a blueprint with the given name doesn't already exist
                if (tribe.Blueprints.Any(x => x.NameId.Equals(bpNameOnly)))
                {
                    await _ctx.RespondAsync($"Invalid blueprint name given. The blueprint {bpNameOnly} already exist within the {tribe.NameId} tribe. Try picking a different name.");
                    return;
                }

                // You can't provide more than one image for a blueprint
                if (_ctx.Message.Attachments.Count > 1)
                {
                    await _ctx.RespondAsync("Invalid number of attachments given. You cannot provide more than one image for a blueprint.");
                    return;
                }

                // Only jpg or png are allowed
                if (_ctx.Message.Attachments.Count == 1)
                {
                    var attachment = _ctx.Message.Attachments[0];

                    if (!ValidateFileIsImg(attachment.FileName, out string ex, out errMsg))
                    {
                        await _ctx.RespondAsync(errMsg);
                        return;
                    }

                    imgUrl = attachment.Url;
                    bpNameWithExtension += $".{ex}";
                }
                else if (@params.Any(p => p.ParamType.Equals(IMG_LINK_PARAM)))
                {
                    var testingUrl = @params.Single(p => p.ParamType.Equals(IMG_LINK_PARAM)).ParamValue;

                    if (!ValidateFileIsImg(testingUrl, out string ex, out errMsg))
                    {
                        await _ctx.RespondAsync(errMsg);
                        return;
                    }

                    imgUrl = testingUrl;
                    bpNameWithExtension += $".{ex}";
                }

                // Create blueprint the blueprint & update db
                var results = await tribe.CreateBlueprint(bpNameOnly, bpNameWithExtension, imgUrl);
                TribeDatabaseContext.Provider.UpdateTribe(tribe);

                // If an error ocurred during the creation of the blueprint report it here
                if (!results.Item1)
                {
                    await _ctx.RespondAsync(results.Item2);
                    return;
                }

                await _ctx.RespondAsync($"Blueprint {bpNameOnly} created!");
                return;
            }

            await _ctx.RespondAsync("Save Command!");
        }

        /// <summary>
        ///     Validates a file url given ex. (https://www.MyWebsite.com/images/MyImage.png) is a valid image file.<br/>
        ///     This is done by getting the last '.' in the name and comparing with the extension after that.<br/>
        ///     @param - fileName, File name given<br/>
        ///     @out param - errMsg, Error message
        ///     Returns whether or not the file given is a valid image to bluequery standards<br/>
        ///     True - Valid<br/>
        ///     False - Invalid
        /// </summary>
        /// <param name="fileUrl"> Given file url </param>
        /// <param name="errMsg"> Error message </param>
        /// <returns></returns>
        private static bool ValidateFileIsImg(in string fileUrl, out string fileExtension , out string errMsg)
        {
            errMsg = string.Empty;

            int extensionIndex = fileUrl.LastIndexOf('.');
            fileExtension = fileUrl.Substring(extensionIndex + 1, fileUrl.Length - extensionIndex - 1);
            if (!(fileExtension.Equals("jpg") || fileExtension.Equals("JPG") || fileExtension.Equals("jpeg") || fileExtension.Equals("JPEG") || fileExtension.Equals("png") || fileExtension.Equals("PNG")))
            {
                errMsg = "Invalid file given. The valid image extensions are as follows:\n" + "`jpg` " + "`JPG` " + "`jpeg` " + "`JPEG` " + "`png` " + "`PNG` ";
                return false;
            }

            return true;
        }
    }
}
