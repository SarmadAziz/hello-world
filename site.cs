[TcmTemplateTitle("OFS Home container components")]
    public class HomePageUtilities : TemplateBase
    {
        public override void Transform(Tridion.ContentManager.Templating.Engine engine, Tridion.ContentManager.Templating.Package package)
        {
            this.Initialize(engine, package);
            Component component = this.GetComponent();
            ItemFields content = new ItemFields(component.Content, component.Schema);

            string extraClass = GetSingleStringValue("extraClass", content);
            List<string> fundList = new List<string>();
            Logger.Debug("FundCount : " + fundList.Count);


            if (extraClass.Length > 0)
            {
                extraClass = " " + extraClass;
            }

            ComponentLinkField componentLinkField = (ComponentLinkField)content["blocks"];
			int componentSize = componentLinkField.Values.Count;
//            Logger.Debug("Aantal: " + componentSize);

            int fundCount = 0;
			bool isOFSFund = false;
            foreach (Component comp in componentLinkField.Values)
            {
//                Logger.Debug("Schema : " + comp.Schema.Title);
                if (String.Compare(comp.Schema.Title, "OFS CS Fund", true) == 0)
                {
					isOFSFund = true;
                    package.PushItem("hasFunds", package.CreateStringItem(ContentType.Text, "true"));
                    break;
                }
            }

			/*
				If it's an "OFS CS Fund" li's should be from top to bottom
				in all other cases where the size is 2 or 3, li's should be 
				from right to left
			*/
			if (isOFSFund)
			{
				ItemFields fields = new ItemFields(comp.Content, comp.Schema);
				package.PushItem("HtmlBlockPackage", package.CreateHtmlItem(GetSingleStringValue("htmlBlock", fields)));

				fundList.Add(GetSingleStringValue("isin", fields));
				Logger.Debug("Isin : " + GetSingleStringValue("isin", fields));
				fundCount = fundCount + 1;
				//package.PushItem("fundCount", package.CreateStringItem(ContentType.Html, fundCount.ToString()));
				
			} else {
				string ulClass = string.Empty;
				StringBuilder output = new StringBuilder();
				
				// makes the correct ul
				switch (componentSize)
				{
					case 2:
						Logger.Debug("Case 2");
						ulClass = " twee" + extraClass;
						output.AppendLine("<ul class=\"kolom" + ulClass + "\">");
						break;
					case 3:
						Logger.Debug("Case 3");
						ulClass = " drie" + extraClass; ;
						output.AppendLine("<ul class=\"kolom" + ulClass + "\">");
						break;
					default:
						Logger.Debug("Default case");
						break;
				}

				// makes corresponding li's
				int i = 1;
				string liClass = string.Empty;
				foreach (Component comp in componentLinkField.Values)
				{
					Logger.Debug("i = " + i);
					if (i == componentSize)
					{
						liClass = " class=\"last\"";
					}
					Logger.Debug("Comp: " + comp.Id);
					if (componentSize == 2 || componentSize == 3 )
					{
						output.AppendLine("<li" + liClass + ">");
					}

					Logger.Debug("Context: " + engine.PublishingContext.RenderContext.ContextItem);
				   
					// what does this do??????  
					string mappingComponentWebDAVURL = "/webdav/CS%20Netherlands/Building%20Blocks%20Management/System%20OFS/system/Schema-CT%20mapping/Schema-CT%20mapping%20homepage.xml";
					if (" klant".Equals(extraClass))
					{ 
						output.Append(RenderComponentPresentation(comp, mappingComponentWebDAVURL, extraClass));
					} else {
						output.Append(RenderComponentPresentation(comp, mappingComponentWebDAVURL));
					}
					
					if (componentSize == 2 || componentSize == 3)
					{
						output.AppendLine("</li>");
					}
					i = i + 1;

				}
				
				// closes the ul
				if (componentSize == 2 || componentSize == 3)
				{
					output.AppendLine("</ul>");
				}
			}
            

			
			
			
			
			
			
			
			
			
			
			
			
			
			
            // make the javascript block for the fundSelection
            Logger.Debug("FundCount : " + fundList.Count);

            if (fundList.Count > 0)
            {
                StringBuilder outputJS = new StringBuilder();
                outputJS.AppendLine("<script type=\"text/javascript\">");
                outputJS.AppendLine("\tvar dict = [];");
                foreach (string isin in fundList)
                {
                    outputJS.AppendLine("\t\tvar isFund-" + isin + "-On = false;");
                }
                outputJS.AppendLine();
                outputJS.AppendLine("\twindow.onload = function(){");
                outputJS.AppendLine("\t\twa.fSetEvent('Top5SelectFunds', [ { switchedFunds: dict, switchMethod:'selectButtonClick' } ] );");
                outputJS.AppendLine("\t}");
                outputJS.AppendLine();

                outputJS.AppendLine("\tFireTagging = function(className, isinCode, productName, method){");
                outputJS.AppendLine("\t\tswitch (isinCode) {");
                foreach (string isin in fundList)
                {
                    outputJS.AppendLine("\t\t\tcase \"" + isin + "\": ");
                    outputJS.AppendLine("\t\t\t\tisFund-" + isin + "-On = (isFund-" + isin + "-On)?false:true;");
                    outputJS.AppendLine("\t\t\t\tbreak;");
                }
               outputJS.AppendLine("\t\t\t\tdefault:");
                outputJS.AppendLine("\t\t\t\tconsole.log(\"default\")");
                outputJS.AppendLine("\t}");
                outputJS.AppendLine();
                outputJS.Append("\tif(className === \"button-alpha orange selectButton\" ");
                foreach (string isin in fundList)
                {
                    outputJS.Append("|| isFund-" +isin + "-On === true ");
                }
                outputJS.AppendLine(") {");
                outputJS.AppendLine("\t\t\twa.fSetEvent('Top5SelectFunds', [ { switchedFunds: [ { sIsinCode: isinCode, sProductName: productName} ], switchMethod: method} ] );");
                outputJS.AppendLine("\t\t\tdict.push({ sIsinCode: isinCode, sProductName: productName});");
                outputJS.AppendLine("\t\t} else {");
                outputJS.AppendLine("\t\tfor (var i = 0, len = dict.length; i < len; i++) {");
                outputJS.AppendLine("\t\t\tif (dict[i].sIsinCode === isinCode) {");
                outputJS.AppendLine("\t\t\t\tdict.splice(i, 1);");
                outputJS.AppendLine("\t\t\t\t}");
                outputJS.AppendLine("\t\t\t}");
                outputJS.AppendLine("\t\t}");
                outputJS.AppendLine("\t}");
                outputJS.AppendLine("");
                outputJS.AppendLine("\tTaggingBuyFunds = function(){");
                outputJS.AppendLine("\t\twa.fSetEvent('Top5BuyFunds', [{ selectedFunds: dict }] );");
                outputJS.AppendLine("</script>");
                package.PushItem("homeJS", package.CreateStringItem(ContentType.Html, outputJS.ToString()));


            }

            package.PushItem("homeBlocks", package.CreateStringItem(ContentType.Html, output.ToString()));

            //Determine if this component is the last on the Page
            if (isLastComponentOnPage(component))
            {
                package.PushItem("lastContainerClass", package.CreateStringItem(ContentType.Text, " lasthome"));
            }

        }
