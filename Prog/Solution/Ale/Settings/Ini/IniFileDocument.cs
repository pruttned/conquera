//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Ale.Settings
{
    public class IniFileDocument
    {
        private static Regex SectionRegex = new Regex(@"^\s*\[(?<Section>[\w\.\:\s]+)\]\s*(;(?<Comment>.*))?$");
        private static Regex ParamRegex = new Regex(@"^(?<Name>[^=]+)\s*=(?<Value>[^;]*)(;(?<Comment>.*))?$");

        public IniFileSectionNodeColection Sections { get; private set; }
        public List<string> Comments { get; private set; }

        public IniFileDocument()
        {
            Sections = new IniFileSectionNodeColection();
            Comments = new List<string>();
        }

        public void Save(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException("file");

            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (IniFileSectionNode section in Sections)
                {
                    WriteComments(writer, section.Comments);

                    writer.Write('[');
                    writer.Write(section.Name);
                    writer.WriteLine(']');

                    foreach (IniFileParameterNode param in section.Parameters)
                    {
                        WriteComments(writer, param.Comments);
                        writer.Write(param.Name);
                        writer.Write('=');
                        writer.WriteLine(param.Value);
                    }
                    writer.WriteLine();
                }

                WriteComments(writer, Comments);
            }
        }

        public void Load(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException("file");

            Load(file, null);
        }

        public void Load(string file, IniFileDocumentLoadSettings settings)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException("file");

            using (StreamReader reader = new StreamReader(file))
            {
                Load(reader, settings);
            }
        }

        public void Load(StreamReader reader)
        {
            Load(reader, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void Load(StreamReader reader, IniFileDocumentLoadSettings settings)
        {
            if (null == reader) throw new ArgumentNullException("reader");

            bool includeComments = null != settings ? settings.IncludeComments : true;
            string sectionNameFilter = null != settings ? settings.SectionNameFilter : null;

            Sections.Clear();
            Comments.Clear();

            int lineNum = 1;
            string line;
            IniFileSectionNode section = null;

            List<string> comments = null;
            if (includeComments)
            {
                comments = new List<string>();
            }

            Regex sectionFilter = null;
            if (!string.IsNullOrEmpty(sectionNameFilter))
            {
                sectionFilter = new Regex(sectionNameFilter, RegexOptions.IgnoreCase);
            }

            while (null != (line = reader.ReadLine()))
            {
                Match sectionMatch = SectionRegex.Match(line);
                if (sectionMatch.Success) //is section
                {
                    string sectionName = sectionMatch.Groups["Section"].Value.Trim();

                    if (null == sectionFilter || sectionFilter.IsMatch(sectionName))
                    {
                        section = EnsureSection(sectionName);

                        if (includeComments)
                        {
                            string comment = sectionMatch.Groups["Comment"].Value.Trim();
                            section.Comments.AddRange(comments);
                            comments.Clear();
                            if (!string.IsNullOrEmpty(comment))
                            {
                                section.Comments.Add(comment);
                            }
                        }
                    }
                    else
                    {
                        section = null;
                        if (includeComments)
                        {
                            comments.Clear();
                        }
                    }
                }
                else
                {
                    Match paramMatch;
                    if (null != section && (paramMatch = ParamRegex.Match(line)).Success)
                    {
                        string name = paramMatch.Groups["Name"].Value.Trim();
                        string value = paramMatch.Groups["Value"].Value.Trim();

                        var param = section.Parameters.Add(name, value);

                        if (includeComments)
                        {
                            string comment = paramMatch.Groups["Comment"].Value.Trim();
                            param.Comments.AddRange(comments);
                            comments.Clear();
                            if (!string.IsNullOrEmpty(comment))
                            {
                                param.Comments.Add(comment);
                            }
                        }
                    }
                    else
                    {
                        string tLine = line.Trim();
                        if (!string.IsNullOrEmpty(tLine))
                        {
                            if (';' == tLine[0])
                            {
                                if (includeComments)
                                {
                                    comments.Add(tLine.Substring(1).Trim());
                                }
                            }
                            else
                            {
                                if (null != section)
                                {
                                    throw new IniFileLoadingException(string.Format("Line {0} has an incorect format", lineNum));
                                }
                            }
                        }
                    }
                }

                lineNum++;
            }

            if (includeComments)
            {
                Comments.AddRange(comments);
            }
        }

        private void WriteComments(StreamWriter writer, List<string> comments)
        {
            foreach (string commment in comments)
            {
                writer.Write("; ");
                writer.WriteLine(commment);
            }
        }

        private IniFileSectionNode EnsureSection(string name)
        {
            IniFileSectionNode section;
            if (!Sections.TryGetNode(name, out section))
            {
                return Sections.Add(name);
            }
            return section;
        }
    }
}
