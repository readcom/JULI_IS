using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Pozadavky.Data;
using Pozadavky.DTO;

namespace Pozadavky.Services
{
    public static class FilesService
    {
        public static string DtbConxString
        {
            get { return CookiesServices.GetCookieValue("DTB"); }
            set { }
        }

        public static List<FilesDTO> GetFilesListByItemID(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var files = (from f in db.Files
                             where f.ItemID == id && f.Smazano == false
                             select new FilesDTO()
                             {
                                 ID = f.ID,
                                 Description = f.Description,
                                 FileName = f.FileName,
                                 FullPath = f.FullPath,
                                 ItemID = f.ItemID ?? 0,
                                 PozadavekID = 0,
                                 Smazano = f.Smazano
                             })
                             .OrderBy(a => a.FileName)
                             .ToList();

                return files;
            }
        }

        public static List<FilesDTO> GetFilesListByPozadavekID(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                var files = (from f in db.Files
                             where f.PozadavekID == id && f.Smazano == false
                             select new FilesDTO()
                             {
                                 ID = f.ID,
                                 Description = f.Description,
                                 FileName = f.FileName,
                                 FullPath = f.FullPath,
                                 ItemID = 0,
                                 PozadavekID = f.PozadavekID ?? 0,
                                 Smazano = f.Smazano
                             })
                             .OrderBy(a => a.FileName)
                             .ToList();

                return files;

            }
        }

        public static void DeleteFile(int id, bool podepsano = false)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var file = db.Files.Find(id);

                if (file != null)
                {
                    if (podepsano)
                    {
                        file.Smazano = true;
                        file.SmazalUzivatel = Constants.ActiveUser;
                        file.DatumSmazani = DateTime.Now;
                    }
                    else
                    {
                        System.IO.File.Delete(file.FullPath);
                        db.Files.Remove(file);
                    }

                    db.SaveChanges();
                }
            }
        }

        public static void DeleteFileByPozadavekAndFileName(int pozId, string filename, string fullPath = "")
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var fileid = (from f in db.Files
                              where f.PozadavekID == pozId & f.FileName == filename
                              select f.ID).SingleOrDefault();

                if (fileid != 0)
                    DeleteFile(fileid);
                else
                {
                    // soubor v databazi neexistuje, muzeme ho smazat
                    System.IO.File.Delete(fullPath);
                }
            }
        }

        public static void DeleteFileByPozadavekId(int pozId, bool podepsano = false)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var fileid = (from f in db.Files
                              where f.PozadavekID == pozId
                              select f.ID).ToList();

                fileid.ForEach(f => DeleteFile(f, podepsano));
                
            }
        }

        public static void DeleteFileByObjAndFileName(int objId, string filename, string fullPath = "")
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var fileid = (from f in db.Files
                              where f.ObjID == objId & f.FileName == filename
                              select f.ID).SingleOrDefault();

                if (fileid != 0)
                    DeleteFile(fileid);
                else
                {
                    // soubor v databazi neexistuje, muzeme ho smazat
                    System.IO.File.Delete(fullPath);
                }
            }
        }

        public static void SaveFile(Stream stream, string path, string fileName, int itemId = 0, int pozId = 0, int objId = 0)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fl = Path.Combine(path, fileName);
            if (System.IO.File.Exists(fl))
                DeleteFileByPozadavekAndFileName(pozId, fl);

            using (var fs = new FileStream(fl, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
            }

            using (var db = new PozadavkyContext(DtbConxString))
            {
                var file = new Data.Files()
                {
                    Description = "",
                    FileName = fileName,
                    FullPath = fl,
                    ItemID = itemId,
                    PozadavekID = pozId,
                    ObjID = objId
                };

                db.Files.Add(file);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Uloží soubor a vrátí jeho ID, pokud soubor existuje tak ho smaze
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="itemId"></param>
        /// <param name="pozId"></param>
        /// <param name="objId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static int SaveFileGetId(Stream stream, string path, string fileName, int itemId = 0, int pozId = 0, int objId = 0, string desc = "")
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fl = Path.Combine(path, fileName);

            if (pozId != 0)
            {
                if (System.IO.File.Exists(fl))
                    DeleteFileByPozadavekAndFileName(pozId, fileName, fl);
            }

            if (objId != 0)
            {
                if (System.IO.File.Exists(fl))
                    DeleteFileByObjAndFileName(objId, fileName, fl);
            }

            using (var fs = new FileStream(fl, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
            }


            using (var db = new PozadavkyContext(DtbConxString))
            {
                var file = new Files()
                {
                    Description = desc,
                    FileName = fileName,
                    FullPath = fl,
                    ItemID = itemId,
                    PozadavekID = pozId,
                    ObjID = objId
                };

                db.Files.Add(file);
                db.SaveChanges();

                return file.ID;
            }
        }

      

        public static void SaveFileDescription(string desc, int id)
        {
            if (!string.IsNullOrEmpty(desc) && (id > 0))
            {
                using (var db = new PozadavkyContext(DtbConxString))
                {
                    var file = db.Files.Find(id);
                    file.Description = desc;
                    db.SaveChanges();
                }
            }
        }

        public static List<FilesDTO> GetFilesListByObjID(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {

                var files = (from f in db.Files
                             where f.ObjID == id && f.Smazano == false
                             select new FilesDTO()
                             {
                                 ID = f.ID,
                                 Description = f.Description,
                                 FileName = f.FileName,
                                 FullPath = f.FullPath,
                                 ItemID = 0,
                                 PozadavekID = f.PozadavekID ?? 0,
                                 Smazano = f.Smazano
                             })
                             .OrderBy(a => a.FileName)
                             .ToList();

                return files;

            }
        }

        public static Files GetFileByID(int id)
        {
            using (var db = new PozadavkyContext(DtbConxString))
            {
                var file = (from f in db.Files
                            where f.ID == id
                            select f).SingleOrDefault();

                return file;
            }
        }

        public static void CopyFilesFromPozadavek2Obj(List<int> pozadavkyId, int objednavkaNewId)
        {
            string juliFile = @"\\juli-app\Pozadavky";
            string mainDirectory = DateTime.Now.Year.ToString();
            string subDir = "O" + DateTime.Now.Year + objednavkaNewId;

            string fullPath = Path.Combine(juliFile, mainDirectory, subDir);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            foreach (var id in pozadavkyId)
            {
                List<FilesDTO> filesList = GetFilesListByPozadavekID(id);

                foreach (var file in filesList)
                {
                    string fileout = Path.Combine(fullPath, file.FileName);

                    using (var stream = new FileStream(file.FullPath, FileMode.Open, FileAccess.Read))
                    {
                        using (var fsout = new FileStream(fileout, FileMode.Create, FileAccess.Write))
                        { stream.CopyTo(fsout); }
                    }

                    using (var db = new PozadavkyContext(DtbConxString))
                    {
                        var updatefile = db.Files.Find(file.ID);
                        updatefile.ObjID = objednavkaNewId;
                        db.SaveChanges();
                    }              
                }

            }
        }

      
    }
}