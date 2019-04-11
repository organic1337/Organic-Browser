using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Organic_Browser.Utils.WebsiteDownloader
{
    /// <summary>
    /// Represents an html document. Includes some useful methods
    /// to work with html documents.
    /// </summary>
    class HtmlDocument
    {
        // The default html encoding
        public const string defaultEncoding = "utf-8";
        // All the elements that does not contain content
        public static readonly string[] NoContentElements = { "area", "base", "basefont", "br", "col", "frame", "hr", "img", "input", "isindex", "meta", "param", "link" };
        // All elements that cannot contain inner elements 
        public static readonly string[] NoInnerElements = { "style", "script", "noscript" };

        public string HtmlCode { get; private set; }                                            // The Html Code                  
        private string CodeWithoutComments { get { return this.GetCodeWithoutComments(); } }    // The html Code without comments
        public Encoding Encoding { get; private set; }                                          // The document encoding

        public HtmlDocument(string htmlCode)
        {
            this.HtmlCode = htmlCode;
            // Set the encoding of the document
            string encodingString;
            List<HtmlElement> metaElements = this.GetElementsBy(e => e.TagName == "meta" && e.Attributes.Keys.Contains("charset")).ToList();
            if (metaElements.Count != 0)            
                encodingString = metaElements[0].Attributes["charset"];            
            else            
                encodingString = defaultEncoding;
            this.Encoding = Encoding.GetEncoding("utf-8");
        }

        /// <summary>
        /// Returns all elements with given tag name.
        /// </summary>
        /// <returns>All elements with given tag name.</returns>
        public IEnumerable<HtmlElement> GetElementsByTagName(string tagName)
        {
            foreach (HtmlElement element in this.GetAllElements())
                if (element.TagName == tagName)
                    yield return element;
        }


        /// <summary>
        /// Returns all the elements in the code that has an attribute as mentiond.
        /// </summary>
        /// <param name="attributeName">Name of the attribute such as 'src', 'href' etc...</param>
        /// <returns> All the elements in the code that has an attribute as mentiond</returns>
        public IEnumerable<HtmlElement> GetElementsByAttribute(string attributeName)
        {
            foreach (HtmlElement element in this.GetAllElements())
                if (element.Attributes.Keys.Any(key => key == attributeName))
                    yield return element;
        }

        /// <summary>
        /// Returns all the elements that the given predicate returns true for them.
        /// </summary>
        /// <param name="predicate">Predicate for filtering the elements </param>
        /// <returns>All the elements that the given predicate returns true for them.</returns>
        public IEnumerable<HtmlElement> GetElementsBy(Predicate<HtmlElement> predicate)
        {
            foreach (HtmlElement element in this.GetAllElements())
                if (predicate(element))
                    yield return element;
        }

        /// <summary>
        /// Returns all elements
        /// </summary>
        /// <returns>All elements in the html code</returns>
        public IEnumerable<HtmlElement> GetAllElements()
        {
            string code = this.CodeWithoutComments;
            code = code.Substring(code.IndexOf("<html"));
            var htmlElement = new HtmlElement(code);
            yield return htmlElement;   // Return the main element (html)

            // Return all the elements one by one
            foreach (HtmlElement element in htmlElement.InnerElements)
                yield return element;
        }


        #region Private Methods
        /// <summary>
        /// Returns the code without the comments
        /// </summary>
        /// <returns></returns>
        private string GetCodeWithoutComments()
        {
            string code = this.HtmlCode;

            while (code.Contains("<!--"))
            {
                int start = code.IndexOf("<!--");
                int end = code.IndexOf("-->");
                code = code.Remove(start, end - start + 3);
            }

            return code;
        }
        #endregion
    }


    /// <summary>
    /// Represents an html element.
    /// </summary>
    class HtmlElement
    {
        public string RawCode { get { return this.ToString(); } }           // The element code as is (with changes)
        public Dictionary<string, string> Attributes { get; private set; }  // The element attributes (can be modified)      
        public string TagName { get; private set; }                         // The element's tag name
        public string Content { get; private set; }                         // The code inside the element
        public int Length { get { return this.RawCode.Length; } }           // The element's length
        public IEnumerable<HtmlElement> InnerElements { get { return this.GetInnerElements(); } }   // All the elements inside this element

        private string code;        // code to work on

        public HtmlElement(string code)
        {
            this.code = code;

            // Assign the fields
            this.TagName = this.GetTagName();
            this.Content = this.GetCodeInside();
            this.Attributes = this.GetAttributes();
        }

        #region private functions
        /// <summary>
        /// Returns the tag name from the code
        /// </summary>
        /// <returns>Tag Name</returns>
        private string GetTagName()
        {
            int firstIndex = this.code.IndexOf('<');    // Start index of the declaration
            int lastIndex = this.code.IndexOf('>');     // End index of the declaration

            string declaration = this.code.Slice(firstIndex + 1, lastIndex);    //for example '<link href="">' -->  'link href=""'
            return declaration.Split(' ')[0];
        }


        /// <summary>
        /// Returns the code inside the element (if contains any code, img element for example does 
        /// not contain any inner code).
        /// </summary>
        /// <returns>The code inside the element</returns>
        private string GetCodeInside()
        {
            if (this.code.Count((c) => c == '<') > 1)
                return this.code.Slice(this.code.IndexOf('>') + 1, this.code.LastIndexOf('<'));
            else
                return "";
        }

        /// <summary>
        /// Returns the attributes of the html element
        /// </summary>
        /// <returns>The attributes of the html element</returns>
        private Dictionary<string, string> GetAttributes()
        {
            int firstIndex = this.code.IndexOf('<');    // Start index of the declaration
            int lastIndex = this.code.IndexOf('>');     // End index of the declaration
            string declaration = this.code.Slice(firstIndex + 1, lastIndex);    //for example '<link href="">' -->  'link href=""'
            string rawAttributes = declaration.Slice(declaration.IndexOf(' ') + 1, declaration.Length); // for example: "src='auto:blank' style='width: 100%'"

            string debug = rawAttributes;

            // Build the attributes dictionary
            var attributes = new Dictionary<string, string>();
            while ((rawAttributes.Contains('"') || rawAttributes.Contains('\'')) && rawAttributes.Contains('='))
            {
                // if raw attributes starts with space, remove it
                rawAttributes = rawAttributes.Strip();

                // get the quatation mark for the attribute for example: "src='auto:blank'" -> "'"
                char quotationMark;
                if (rawAttributes.Contains('"') && rawAttributes.Contains('\''))
                {
                    if (rawAttributes.IndexOf('"') < rawAttributes.IndexOf('\''))
                        quotationMark = '"';
                    else
                        quotationMark = '\'';
                }
                else if (rawAttributes.Contains('"'))
                    quotationMark = '"';
                else
                    quotationMark = '\'';

                // Get the key and the value of the attribute
                int equalsIndex = rawAttributes.IndexOf('=');
                int spaceIndex = rawAttributes.IndexOf(' ');
                string key;
                if (spaceIndex != -1 && spaceIndex < equalsIndex)
                    key = rawAttributes.Slice(spaceIndex + 1, equalsIndex);
                else
                    key = rawAttributes.Slice(0, equalsIndex);   // key of the attribute for example src or href
                int quotationMarkIndex = rawAttributes.IndexOf(quotationMark);
                int nextQuotationMarkIndex = rawAttributes.IndexOf(quotationMark, quotationMarkIndex + 1);
                string value = rawAttributes.Slice(quotationMarkIndex + 1, nextQuotationMarkIndex);  // the value of the attribute for example 'src="auto:blank"' -> auto:blank

                key = key.ToLower();        // Make the key lower-case
                attributes[key] = value;    // Add the attribute to the attributes dictionary 
                rawAttributes = rawAttributes.Slice(nextQuotationMarkIndex + 1, rawAttributes.Length);
            }

            return attributes;
        }

        /// <summary>
        /// Whether the element can contain content or not .
        /// img element for example will return false because cannot
        /// contain content. p will return true.
        /// </summary>
        private bool IsNonContentElement
        {
            get
            {
                return HtmlDocument.NoContentElements.Any(tag => tag == this.TagName);
            }
        }

        /// <summary>
        /// Returns all elements inside the currentElement
        /// </summary>
        /// <returns>All elements inside the current element</returns>
        private IEnumerable<HtmlElement> GetInnerElements()
        {
            string code = this.Content;                 // Code to work on

            var elementIndexes = new Stack<int>();      // Used to temporarely store the indexes of where the elements start
            int currentIndex = 0;                       // Current index in the html code
            int lastIndex = -1;                         // Last index checked
            string currentTagName;

            while (code.IndexOf('<', currentIndex) != -1)
            {
                int lessCharacterIndex = code.IndexOf("<", currentIndex);
                // In case closing Tag
                if (code[lessCharacterIndex + 1] == '/')
                {
                    int firstIndex = elementIndexes.Pop();                  // First index of the element
                    lastIndex = code.IndexOf('>', lessCharacterIndex);      // Last index of the element

                    var htmlElement = new HtmlElement(code.Slice(firstIndex, lastIndex + 1));
                    yield return htmlElement;
                }
                // In case opening tag
                else
                {
                    // Get the tag name
                    currentTagName = Regex.Match(code.Substring(lessCharacterIndex + 1), @"([a-z]*)[> ]").Groups[1].Value;

                    // In case the element can't contain any inner elements
                    if (HtmlDocument.NoInnerElements.Contains(currentTagName))
                    {
                        string closingTag = string.Format("</{0}>", currentTagName);
                        lastIndex = code.IndexOf(closingTag, lessCharacterIndex) + closingTag.Length;
                        var htmlElement = new HtmlElement(code.Slice(lessCharacterIndex, lastIndex));
                        yield return htmlElement;
                    }
                    // In case the element can't have content (such as img elements)
                    else if (HtmlDocument.NoContentElements.Contains(currentTagName))
                    {
                        lastIndex = code.IndexOf('>', lessCharacterIndex);
                        var htmlElement = new HtmlElement(code.Slice(lessCharacterIndex, lastIndex + 1));
                        yield return htmlElement;
                    }
                    // In case the element can have content (such as div element)
                    else
                    {
                        int firstIndex = lessCharacterIndex;
                        elementIndexes.Push(firstIndex);

                        lastIndex = firstIndex + 1;
                    }
                }

                currentIndex = lastIndex;   // Continue checking from the next relevant index
            }
        }
        #endregion

        /// <summary>
        /// Format code from the element's attributes
        /// </summary>
        /// <returns>The raw html code</returns>
        public override string ToString()
        {
            string declaration = string.Format("<{0}", this.TagName);
            foreach (KeyValuePair<string, string> pair in this.Attributes)
            {
                declaration += string.Format(" {0}=\"{1}\"", pair.Key, pair.Value);
            }
            if (this.IsNonContentElement)
                return declaration + "/>";

            declaration += ">";
            return string.Format("{0}{1}</{2}>", declaration, this.Content, this.TagName);

        }
    }
}
