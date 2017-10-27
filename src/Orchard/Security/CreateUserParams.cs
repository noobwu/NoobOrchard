namespace Orchard.Security {
    //TEMP: Add setters, provide default constructor and remove parameterized constructor
    public class CreateUserParams {
        private readonly string _username;
        private readonly string _password;
        private readonly string _email;
        private readonly string _passwordQuestion;
        private readonly string _passwordAnswer;
        private readonly bool _isApproved;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="passwordQuestion"></param>
        /// <param name="passwordAnswer"></param>
        /// <param name="isApproved"></param>
        public CreateUserParams(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved) {
            _username = username;
            _password = password;
            _email = email;
            _passwordQuestion = passwordQuestion;
            _passwordAnswer = passwordAnswer;
            _isApproved = isApproved;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Username {
            get { return _username; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Password {
            get { return _password; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Email {
            get { return _email; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordQuestion {
            get { return _passwordQuestion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PasswordAnswer {
            get { return _passwordAnswer; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsApproved {
            get { return _isApproved; }
        }
    }
}