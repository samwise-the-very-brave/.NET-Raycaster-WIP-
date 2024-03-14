namespace DDA
{
    struct DebugLog
    {
        public string Value { get; init; }
        public string Caption { get; init; }

        public DebugLog(string value, string caption)
        {
            Value = value;
            Caption = caption;
        }
    }
}
