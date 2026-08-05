namespace jCommunicator
{
    public struct LinuxFileInfo
    {
        public string Permissions { get; init; }
        public int HardLinks { get; init; }
        public string Owner { get; init; }
        public string Group { get; init; }
        public long Size { get; init; }
        public DateTimeOffset LastWriteTime { get; init; }
        public string Name { get; init; }
        public bool IsDirectory => Permissions.StartsWith("d");

        public override string ToString()
        {
            return $"{Name} ({Size} bytes, {LastWriteTime})";
        }
    }
}
