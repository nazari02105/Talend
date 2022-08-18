using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ETLLibrary.Database.Gataways;
using ETLLibrary.Database.Models;
using ETLLibrary.Database.Utils;
using ETLLibrary.Interfaces;

namespace ETLLibrary.Database.Managers
{
    public class CsvDatasetManager : ICsvDatasetManager
    {
        private CsvGateway _gateway;
        private ICsvSerializer _serializer;

        public CsvDatasetManager(EtlContext context, ICsvSerializer serializer)
        {
            _serializer = serializer;
            _gateway = new CsvGateway(context);
        }

        public void SaveCsv(Stream stream, string username, string fileName, CsvInfo info, long fileLength)
        {
            EnsureDirectoryCreated(CsvConfigurator.Path);
            EnsureUserDirectoryCreated(CsvConfigurator.Path, username);

            if (!File.Exists(CsvConfigurator.GetFilePath(username, fileName)))
            {
                _gateway.AddDataset(username, fileName, info);

                var content = FormFileReader.Read(stream, fileLength);
                File.WriteAllText(CsvConfigurator.GetFilePath(username, fileName), content);
            }
            else
            {
                throw new Exception("File with this name already exists.");
            }
        }
        private void EnsureDirectoryCreated(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void EnsureUserDirectoryCreated(string path, string username)
        {
            if (!Directory.Exists(path + "/" + username))
            {
                Directory.CreateDirectory(path + "/" + username);
            }
        }

        public List<string> GetCsvFiles(string username)
        {
            return _gateway.GetUserDatasets(username);
        }

        public List<List<string>> GetCsvContent(User user, string name)
        {
            var csv = _gateway.GetDataset(name, user.Id);
            if (csv != null && FileExists(user.Username, csv.FileName))
            {
                return _serializer.Serialize(csv, CsvConfigurator.GetFilePath(user.Username, csv.FileName));
            }

            return null;
        }

        private bool FileExists(string username, string fileName)
        {
            return Directory.Exists(CsvConfigurator.Path) && Directory.Exists(CsvConfigurator.GetUserDirectoryPath(username)) &&
                   File.Exists(CsvConfigurator.Path + "/" + username + "/" + fileName);
        }

        public bool DeleteCsv(User user, string name)
        {
            var csv = _gateway.GetDataset(name, user.Id);
            if (csv == null) return false;
            File.Delete(CsvConfigurator.GetFilePath(user.Username, csv.FileName));
            _gateway.DeleteDataset(name, user.Id);
            return true;
        }
    }
}