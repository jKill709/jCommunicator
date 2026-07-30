using System;
using System.IO;
using Xunit;

namespace jCommunicator.Tests.Integration
{
    public class HubFileOperationTests : CommunicatorTestBase
    {
        private static string NewHubPath()
        {
            return $"/tmp/jCommunicatorTest_{Guid.NewGuid():N}.txt";
        }

        [Fact]
        public void HubFileExists_NewFile_ReturnsTrue()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            try
            {
                CreateHubFile(_communicator, file);

                Assert.True(_communicator.HubFileExists(file));
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }

        [Fact]
        public void HubFileExists_MissingFile_ReturnsFalse()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            Assert.False(_communicator.HubFileExists(file));
        }

        [Fact]
        public void HubFileLastModified_NewFile_IsRecent()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            try
            {
                CreateHubFile(_communicator, file);

                DateTime modified = _communicator.HubFileLastModified(file);

                TimeSpan age = DateTime.Now - modified;

                Assert.True(age.TotalSeconds < 10);
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }

        [Fact]
        public void GetListOfHubFiles_ReturnsCreatedFile()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            try
            {
                CreateHubFile(_communicator, file);

                int lastSlash = file.LastIndexOf('/');

                string filepath = file[..lastSlash];
                string filename = file[(lastSlash + 1)..];

                string[] files = _communicator.GetListOfHubFiles(filepath!, ".txt");

                Assert.Contains(filepath + '/' + filename, files);
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }

        [Fact]
        public void DeleteHubFile_RemovesFile()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            CreateHubFile(_communicator, file);

            Assert.True(_communicator.HubFileExists(file));

            _communicator.DeleteHubFile(file);

            Assert.False(_communicator.HubFileExists(file));
        }

        [Fact]
        public void DeleteHubFile_MissingFile_DoesNotThrow()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            var ex = Record.Exception(() =>
            {
                _communicator.DeleteHubFile(file);
            });

            Assert.Null(ex);
        }

        [Fact]
        public void MoveHubFile_RenamesFile()
        {
            _communicator!.Connect();

            string source = NewHubPath();
            string destination = NewHubPath();

            try
            {
                CreateHubFile(_communicator, source);

                _communicator.MoveHubFile(source, destination);

                Assert.False(_communicator.HubFileExists(source));
                Assert.True(_communicator.HubFileExists(destination));
            }
            finally
            {
                if (_communicator.HubFileExists(source))
                    _communicator.DeleteHubFile(source);

                if (_communicator.HubFileExists(destination))
                    _communicator.DeleteHubFile(destination);
            }
        }

        [Fact]
        public void MoveHubFile_InvalidSource_ReturnsFalse()
        {
            _communicator!.Connect();

            string source = NewHubPath();
            string destination = NewHubPath();

            Assert.False(_communicator.MoveHubFile(source, destination));
        }

        [Fact]
        public void HubFileNames_WithSpaces_Work()
        {
            _communicator!.Connect();

            string file = $"/tmp/Test File {Guid.NewGuid():N}.txt";

            try
            {
                CreateHubFile(_communicator, $"\"{file}\"");

                Assert.True(_communicator.HubFileExists(file));
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }

        [Fact]
        public void HubEmptyFile_Exists()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            try
            {
                _communicator.ExecuteHubCommand($"touch {file}");

                Assert.True(_communicator.HubFileExists(file));
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }

        [Fact]
        public void HubLargeFile_Exists()
        {
            _communicator!.Connect();

            string file = NewHubPath();

            try
            {
                _communicator.ExecuteHubCommand($"dd if=/dev/zero of={file} bs=1M count=10 status=none");

                Assert.True(_communicator.HubFileExists(file));
            }
            finally
            {
                if (_communicator.HubFileExists(file))
                    _communicator.DeleteHubFile(file);
            }
        }
    }
}