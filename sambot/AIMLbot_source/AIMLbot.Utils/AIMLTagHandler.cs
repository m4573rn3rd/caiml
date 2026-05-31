using System.Xml;

namespace AIMLbot.Utils;

public abstract class AIMLTagHandler : TextTransformer
{
	public bool isRecursive = true;

	public User user;

	public SubQuery query;

	public Request request;

	public Result result;

	public XmlNode templateNode;

	public AIMLTagHandler(Bot bot, User user, SubQuery query, Request request, Result result, XmlNode templateNode)
		: base(bot, templateNode.OuterXml)
	{
		this.user = user;
		this.query = query;
		this.request = request;
		this.result = result;
		this.templateNode = templateNode;
		this.templateNode.Attributes.RemoveNamedItem("xmlns");
	}

	public AIMLTagHandler()
	{
	}

	public static XmlNode getNode(string outerXML)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(outerXML);
		return xmlDocument.FirstChild;
	}
}
