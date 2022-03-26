using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SearchAndCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0)
                {
                    string dir = args[0];
                    string Copyto = args[1];

                    if (!Directory.Exists(dir))
                    {
                        Debug.WriteLine("フォルダが見つかりませんでした。");
                        return;
                    }

                    try { Directory.CreateDirectory(Copyto); }catch { }

                    Console.WriteLine("コピー先：" + Copyto);

                    List<string> filenames = new List<string> { };
                    List<string> renameto = new List<string> { };

                    int count = 0;

                    foreach(string arg in args)
                    {
                        if (args[count] == "-n")
                        {
                            break;
                        }
                        else
                        {
                            filenames.Add(arg);
                            count++;
                        }
                    }

                    if(filenames.Count != args.Length)
                    {
                        for (int i = count + 1; i < args.Length; i++)
                        {
                            renameto.Add(args[i]);
                        }
                    }

                    Console.WriteLine("検索ディレクトリ：" + dir + " を検索しています...");
                    List<string> files = GetAllFiles(dir);

                    Console.WriteLine(files.Count + "件のファイルが見つかりました。");
                    Console.WriteLine("ファイルを検索中...");
                    List<string> result = SearchFromList(files, filenames);

                    if (result.Count != 0)
                    {
                        Console.WriteLine("\n" + result.Count + "件のファイルが見つかりました。");
                        Console.WriteLine(Copyto + "にファイルをコピー中...");

                        int i = 0;

                        foreach(string res in result)
                        {
                            Console.CursorLeft = 0;
                            Console.Write((i + 1) + "/" + result.Count + " " + (Math.Ceiling(((float)i / result.Count) * 100)) + "% " + res);
                            
                            string filename = "";

                            if (renameto.Count != 0)
                            {
                                filename = Copyto + "\\" + Path.GetFileName(renameto[i]);
                            }
                            else
                            {
                                filename = Copyto + "\\" + Path.GetFileName(res);
                            }

                            try
                            {
                                File.Copy(res, filename);
                            }
                            catch (Exception)
                            {

                            }

                            i++;
                        }

                        Console.WriteLine("\nコピーが完了しました。");
                    }
                    else
                    {
                        Console.WriteLine("\n" + result.Count + "件のファイルが見つかりましたが" + (filenames.Count - result.Count) + "個のファイルは見つかりませんでした。");
                    }
                }
                else
                {
                    Console.WriteLine("引数を指定してください。");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nエラーが発生しました。");
                Console.WriteLine(e);
            }
        }

        public static List<string> SearchFromList(List<string> files,List<string> filename)
        {
            List<string> filenamelist = new List<string> { };

            for (int i = 0; i<files.Count; i++)
            {
                Console.CursorLeft = 0;
                Console.Write(i+1 + "/" + files.Count + " ");
                Console.Write((Math.Truncate(((float)i/files.Count)*100)+1.0) + "% ");

                for (int s = 0; s < filename.Count; s++)
                {
                    if (Path.GetFileName(files[i]) == filename[s])
                    {
                        Console.CursorLeft = 0;
                        filenamelist.Add(files[i]);
                        Console.Write(Path.GetFileName(files[i]));
                    }
                }
            }

            return filenamelist;
        }

        /*
         * サブディレクトリも含め全てのファイル名を取得する関数
         * 
         * 引用元：【C#】ドライブ直下からのファイルリスト取得について
         *        https://qiita.com/OneK/items/8b0d02817a9f2a2fbeb0
         */
        public static List<string> GetAllFiles(string DirPath)
        {
            List<string> lstStr = new List<string>();    // 取得したファイル名を格納するためのリスト
            string[] strBuff;   // ファイル名とディレクトリ名取得用

            try
            {
                // ファイル名取得
                strBuff = Directory.GetFiles(DirPath);        // 探索範囲がルートフォルダで時間が掛かるため、テキスト形式のファイルのみ探索
                foreach (string file in strBuff)
                {
                    lstStr.Add(file);
                }

                // ディレクトリ名の取得
                strBuff = Directory.GetDirectories(DirPath);
                foreach (string directory in strBuff)
                {
                    List<string> lstBuff = GetAllFiles(directory);    // 取得したディレクトリ名を引数にして再帰
                    lstBuff.ForEach(delegate (string str)
                    {
                        lstStr.Add(str);
                    });
                }
            }
            catch (Exception e)
            {
                // 主に発生する例外は、システムフォルダ等で発生するアクセス拒否
                //        例外名：System.UnauthorizedAccessException
                Console.WriteLine(e);
            }

            // 取得したファイル名リストを呼び出し元に返す
            return lstStr;

        }
        
    }
}
