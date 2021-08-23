using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CheckHardwareInfo
{
    class JsonWriter : WriterBase
    {
        public JsonWriter(object t, string path) : base(t, path)
        {
            //nothing
        }

        protected override string StartElementPatern(string element)
        {
            return "\"" + element + "\":{";
        }

        protected override string EndElementPatern(string element)
        {
            return "}";
        }

        protected override void WriteStartElementProperty(string element)
        {
            string e = "";
            if (stack.Count() != 0)
            {
                e = "\n";
            }

            for (int i = 0; i < stack.Count(); i++)
            {
                e += "\t";
            }
            e += "\"" + element + "\": \"";

            string end = "\n";
            for (int i = 0; i < stack.Count(); i++)
            {
                end += "\t";
            }
            stack.Push(end + "\",");

            Stream.Write(e);
        }

        protected override void WriteEndElementProperty()
        {
            base.WriteEndElement();
        }

        protected override void WriteStartRoutine()
        {
            Stream.Write("{");
            stack.Push("\n}");
        }

        protected override void WriteEndRoutine()
        {
            Stream.Write(stack.Pop());
        }
    }

    class YamlWriter : WriterBase
    {
        public YamlWriter(object t, string path) : base(t, path)
        {
            //nothing
        }

        protected override string StartElementPatern(string element)
        {
            return element + " : ";
        }

        protected override string EndElementPatern(string element)
        {
            return "";
        }
    }

    class HtmlWriter : WriterBase
    { 
        public HtmlWriter(object t, string path) : base(t, path)
        {
            //nothing
        }

        protected override void WriteStartRoutine()
        {
            string routine = "<html>\n\t<head>\n\t\t" +
                "<style type=\"text / css\">" +
                "body { font-family: Tahoma, Arial, Verdana; font-size:12px; }h1 " +
                "{ font-size:14px;  }td, th { padding:3px; border:1px solid black; }th { font-weight:bold; }</style>" +
                "\t\t<title>Document</title>" +
                "\n\t</head>" +
                "\n\t<body>" +
                "\n\t\t<table cellspacing=\"0\" cellpadding=\"0\" border=\"1\">\n" +
                "\t\t\t<tr>\n" +
                "\t\t\t\t<th>Type</th>\n" +
                "\t\t\t\t<th>Value</th>\n" + 
                "\t\t\t</tr>\n";

            Stream.Write(routine);
        }

        protected override void WriteEndRoutine()
        {
            string routine = "\t\t</table>\n" + "\t</body>\n" + "</html>";
            Stream.Write(routine);
        }

        protected override void WriteStartElement(string elementName)
        {
            string e = "\t\t\t<tr>\n" +
                "\t\t\t\t<td style=\"font-weight: bold; padding-left: " + (15 * stack.Count()).ToString() + "px;\">" +
                elementName +
                "</td>\n" +
                "\t\t\t\t<td></td>\n" +
                "\t\t\t</tr>\n";

            Stream.Write(e);

            stack.Push("e");
        }

        protected override void WriteStartElementProperty(string element)
        {
            string e = "\t\t\t<tr>\n" +
                "\t\t\t\t<td style=\"padding-left: " + (15 * stack.Count()).ToString() + "px; \">" + element +
                "</td>\n" +
                "\t\t\t\t<td>";

            Stream.Write(e);

            stack.Push("e");
        }

        protected override void WriteCharacters(string c)
        {
            Stream.Write(c);
        }

        protected override void WriteEndElementProperty()
        {
            stack.Pop();

            string e = "</td>\n" +
                "\t\t\t</tr>\n";

            Stream.Write(e);
        }

        protected override void WriteEndElement()
        {
            stack.Pop();
        }
    }

    class XmlWriter : WriterBase
    {
        public XmlWriter(object t, string path) : base(t, path)
        {
            //nothing
        }

        protected override string StartElementPatern(string element)
        {
            return "<" + element + ">";
        }

        protected override string EndElementPatern(string elementName)
        {
            return "</" + elementName + ">";
        }

        protected override void WriteStartRoutine()
        {
            string startRoutine = "<?xml version=\"1.0\"?>\n";
            Stream.Write(startRoutine);
        }
    }

    public class WriterBase
    {
        protected Stack<string> stack = new Stack<string>();
        protected object tempObject;
        protected StreamWriter Stream;

        public static WriterBase CreateWriterInstance<T>(T t, string filePath)
        {
            if(t == null)
            {
                return null;
            }
            switch (Path.GetExtension(filePath))
            {
                case ".xml":
                    {
                        return new XmlWriter(t, filePath);
                    }
                case ".html":
                    {
                        return new HtmlWriter(t, filePath);
                    }
                case ".json":
                    {
                        return new JsonWriter(t, filePath);
                    }
                case ".yaml":
                    {
                        return new YamlWriter(t, filePath);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        protected WriterBase(object t, string path)
        {
            tempObject = t;
            Stream = new StreamWriter(path);
        }

        protected virtual void WriteStartElement(string elementName)
        {
            string e = "";
            if (stack.Count() != 0)
            {
                e = "\n";
            }

            for (int i = 0; i < stack.Count(); i++)
            {
                e += "\t";
            }
            e += StartElementPatern(elementName);

            string end = "\n";
            for (int i = 0; i < stack.Count(); i++)
            {
                end += "\t";
            }
            stack.Push(end + EndElementPatern(elementName));

            Stream.Write(e);
        }

        protected virtual void WriteEndElement()
        {
            Stream.Write(stack.Pop());
        }

        protected virtual void WriteCharacters(string c)
        {
            string e = stack.Pop().Replace("\t", "").Replace("\n", "");
            stack.Push(e);

            Stream.Write(c);
        }

        protected virtual void WriteStartElementProperty(string element)
        {
            WriteStartElement(element);   
        }

        protected virtual void WriteEndElementProperty()
        {
            WriteEndElement();
        }

        protected virtual void WriteStartRoutine()
        {

        }

        protected virtual void WriteEndRoutine()
        {

        }

        protected virtual string StartElementPatern(string element)
        {
            return "";
        }

        protected virtual string EndElementPatern(string element)
        {
            return "";
        }

        public void WriteToFile()
        {
            WriteStartRoutine();

            WriteStartElement(tempObject.GetType().Name);
            BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.GetProperty;
            PropertyInfo[] tempObjectInfos = tempObject.GetType().GetProperties(bindingFlags);
            foreach(var info in tempObjectInfos)
            {
                object item = info.GetValue(tempObject);

                if (item == null)
                    continue;

                PropertyInfo[] itemInfos = item.GetType().GetProperties(bindingFlags);

                if(itemInfos.Count() == 1)
                {
                    IEnumerable<object> objectLists = itemInfos[0].GetValue(item) as IEnumerable<object>;
                    WriteStartElement(item.GetType().Name);
                    foreach(var listOfObj in objectLists)
                    {
                        PropertyInfo[] listOfObjInfo = listOfObj.GetType().GetProperties(bindingFlags);
                        WriteStartElement(listOfObj.GetType().Name);
                        foreach (var itemInfo in listOfObjInfo)
                        {
                            WriteStartElementProperty(itemInfo.Name);
                            WriteCharacters(itemInfo.GetValue(listOfObj).ToString());
                            WriteEndElementProperty();
                        }
                        WriteEndElement();
                    }
                    WriteEndElement();
                }
                else
                {
                    WriteStartElement(item.GetType().Name);
                    foreach (var itemInfo in itemInfos)
                    {
                        WriteStartElementProperty(itemInfo.Name);
                        WriteCharacters(itemInfo.GetValue(item).ToString());
                        WriteEndElementProperty();
                    }
                    WriteEndElement();
                }
            }

            WriteEndElement();

            WriteEndRoutine();

            Stream.Flush();
            Stream.Dispose();
            Stream.Close();
        }
    }
}
