﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.XPath;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace Movie
{
    internal class DataProvider
    {
        public DataProvider() 
        {
            
        }   

        internal Dictionary<int, HashSet<string>> GetActorsCodes(string fileName) 
        {
            Dictionary<int, HashSet<string>> result = new Dictionary<int, HashSet<string>>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {

                    //string[] values = line.Split('\t',4);
                    //14325969 
                    //14325926
                    //14325870
                    //14329098
                    //14305569
                    string strId = line.Substring(2, 7);
                    string s = line.Substring(10);
                    int i = s.IndexOf('\t');
                    string c = s.Substring(i + 1);
                    string strPersonId = c.Substring(2, 7);
                    string role = c.Substring(10, 2);

                    if (/*line.Contains("actor") || line.Contains("director") || line.Contains("actress")*/
                        role.Equals("ac") || role.Equals("di")
                        ) 
                    {
                        
                        
                        int id = int.Parse(strId);
                        string personId = role + int.Parse(strPersonId);

                        if (result.ContainsKey(id)) result[id].Add(personId);
                        else
                        {
                            result.Add(id, new HashSet<string>() { personId });
                        }
                    }
                }

                return result;
            }
        }

        internal Task ReadMoviesCodesAsync(string fileName, BlockingCollection<string> lines) 
        {
            return Task.Factory.StartNew(() => 
            {
                ReadMoviesCodes(fileName, lines);
                lines.CompleteAdding();
            }, TaskCreationOptions.LongRunning);
        }

        internal void ReadMoviesCodes(string fileName, BlockingCollection<string> lines)
        {
            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;
                
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    int id = int.Parse(line.Substring(2, 7));
                    int i = line.Substring(10).IndexOf('\t');
                    string s = line.Substring(i + 1);
                    int k = s.IndexOf('\t');
                    string title = s.Substring(0,k);
                    string l = s.Substring(k + 1);
                    int ind = l.IndexOf('\t');
                    string lang = l.Substring(0, ind);
                    
                    if (!lang.Equals("RU") || !lang.Equals("EN")) continue;

                    lines.TryAdd($"{id},{title}");
                }
            }
        }

        public bool IsMovieLineValid(string line) 
        {
            
        }

        internal Dictionary<int, double> GetMoviesRatings(string fileName)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                { 

                    int id = int.Parse(line.Substring(2,7));
                    double rate = double.Parse(line.Substring(10, 3), new NumberFormatInfo()
                    {
                        NumberDecimalSeparator = "."
                    });

                    result.Add(id, rate);
                }
            }

            return result;
        }

        internal Dictionary<int, string> GetActorsNames(string fileName)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    int id = int.Parse(line.Substring(2,7));
                    
                    string s = line.Substring(10);
                    string name = s.Substring(0, s.IndexOf('\t'));

                    result.TryAdd(id, name);
                }

            }
            return result;
        }

        internal Dictionary<int, int> GetCodesLinks(string fileName)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    int i = line.IndexOf(',');
                    int movieLensId = int.Parse(line.Substring(0, i));
                    string s = line.Substring(i + 1);
                    int imdbId = int.Parse(s.Substring(0, s.IndexOf(',')));

                    result.Add(imdbId, movieLensId);
                }
            }

            return result;
        }

        internal Dictionary<int, string> GetTags(string fileName)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                { 
                    int i = line.IndexOf(',');
                    int id = int.Parse(line.Substring(0, i));
                    string tag = line.Substring(i + 1);

                    result.Add(id, tag);
                }
            }

            return result;
        }

        internal Dictionary<int, HashSet<int>> GetMoviesTags(string fileName)
        {
            Dictionary<int, HashSet<int>> result = new Dictionary<int, HashSet<int>>();

            using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
            {
                string line = string.Empty;

                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    double d = double.Parse(line.Substring(line.IndexOf('.') - 1), new NumberFormatInfo() 
                    {
                        NumberDecimalSeparator = "."
                    });
                    if (d <= 0.5) continue;

                    int i = line.IndexOf(',');

                    int movieId = int.Parse(line.Substring(0, i));
                    string s = line.Substring(i + 1);
                    int tagId = int.Parse(s.Substring(0, s.IndexOf(',')));

                    if (result.ContainsKey(movieId)) result[movieId].Add(tagId);
                    else
                    {
                        result.Add(movieId, new HashSet<int>() { tagId });
                    }
                }
            }

            return result;
        }
    }
}
