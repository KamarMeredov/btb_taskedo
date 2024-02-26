namespace BlogPlatform.Helpers
{
    public class ObjectNotFoundException : Exception
    {
        private readonly string _varName;
        public ObjectNotFoundException(string varName) 
        {
            _varName = varName;
        }

        public override string Message => $"Variable '{_varName}' is not found (is set to null)";

    }
}
