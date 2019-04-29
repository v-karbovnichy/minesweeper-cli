namespace minesweeper_cli
{
    internal class UserInput
    {
        public UserAction UserAction { get; private set; }
        
        public bool DidUserAskToExit => UserAction == UserAction.Exit;

        public int Row { get; private set; }
        public int Col { get; private set; }

        private UserInput()
        {
            UserAction = UserAction.Unknown;
        }

        public static UserInput FromString(string rawUserInput)
        {
            var result = new UserInput();
            if (rawUserInput == "exit")
            {
                result.UserAction = UserAction.Exit;
            } 
            else if (rawUserInput.StartsWith("o "))
            {
                if (result.TryFillCoords(rawUserInput))
                {
                    result.UserAction = UserAction.OpenSpace;
                };
            }
            else if (rawUserInput.StartsWith("f "))
            {
                if (result.TryFillCoords(rawUserInput))
                {
                    result.UserAction = UserAction.ToggleFlag;
                }
            }

            return result;
        }

        private bool TryFillCoords(string rawUserInput)
        {
            var pieces = rawUserInput.Split(' ');
            
            //TODO (OPTIONAL) check for exact 2 args

            if (int.TryParse(pieces[1], out var row)
                && int.TryParse(pieces[2], out var col))
            {
                Row = row;
                Col = col;
                return true;
            }
            
            return false;
        }
    }
}