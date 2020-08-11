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

namespace BlueQuery.Commands
{
    public class BlueprintCommands
    {
        const string NAME_PARAM = " -name ";
        const string CREATE_PARAM = " -create ";
        const string TRIBE_PARAM = " -tribe ";
        const string IMAGE_PARAM = " -img ";

        public readonly string[] SINGLE_USE_PARAMS = { NAME_PARAM, TRIBE_PARAM, CREATE_PARAM, IMAGE_PARAM };

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
                // Validate and process the given name
                string bpName = (@params.Single(p => p.ParamType.Equals(CREATE_PARAM)).ParamValue);
                if (!StrParseUtil.ValidateName(ref bpName, out errMsg))
                {
                    await _ctx.RespondAsync(errMsg);
                    return;
                }                

                // Check to make sure the given tribe exist
                if (!TribeDatabaseContext.Provider.DoesTribeExist(@params.Single(t => t.ParamType.Equals(TRIBE_PARAM)).ParamValue, out Tribe tribe, out errMsg))
                {
                    await _ctx.RespondAsync(errMsg);
                    return;
                }

                // Check to make sure a blueprint with the given name doesn't already exist
                if (tribe.Blueprints.ContainsKey(bpName))
                {
                    await _ctx.RespondAsync($"Invalid blueprint name given. The blueprint {bpName} already exist within the {tribe.NameId}. Try picking a different name.");
                    return;
                }

                // Only one image is allowed to be provided for a single blueprint
                if (_ctx.Message.Attachments.Count > 1)
                {
                    await _ctx.RespondAsync("Invalid number of attachments given. A blueprint can only have one image.");
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

                    int extensionIndex = attachment.FileName.LastIndexOf('.');
                    string ex = attachment.FileName.Substring(extensionIndex + 1, attachment.FileName.Length - extensionIndex - 1);                   
                    if (!(ex.Equals("jpg") || ex.Equals("JPG") || ex.Equals("jpeg") || ex.Equals("JPEG") || ex.Equals("png") || ex.Equals("PNG")))
                    {
                        await _ctx.RespondAsync("Invalid file given. The valid image extensions are as follows:\n" + "`jpg` " + "`JPG` " + "`jpeg` " + "`JPEG` " + "`png` " + "`PNG` ");
                        return;
                    }

                    await tribe.CreateBlueprint(attachment.Url, bpName + "." + ex);
                    await _ctx.RespondAsync("BP Created");
                    return;
                }

                // Create blueprint the blueprint
                

                await _ctx.RespondAsync("CREATE-PARAM detected.");
                return;
            }




            await _ctx.RespondAsync("Save Command!");
        }
    }
}
