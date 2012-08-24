#region "Imports"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Development.Materia.Database
{
    /// <summary>
    /// SQL command statement parsing class.
    /// </summary>
    public class CommandParser
    {

        #region "constructors"

        /// <summary>
        /// Creates a new instance of CommandParser.
        /// </summary>
        /// <param name="que"></param>
        public CommandParser(Que que)
        { _que = que; Parse(); }

        #endregion

        #region "properties"

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandStatementCollection _commandstatements = null;

        /// <summary>
        /// Gets the current lists of evaluated command statements based on the associated Que object.
        /// </summary>
        public CommandStatementCollection CommandStatements
        {
            get 
            {
                if (_commandstatements == null) _commandstatements = new CommandStatementCollection(this);
                return _commandstatements; 
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Que _que = null;

        /// <summary>
        /// Gets the current associated que object for the current command parser.
        /// </summary>
        public Que Que
        {
            get { return _que; }
        }

        #endregion

        #region "methods"

        private string CleanStatement(string sql)
        {
            string _cleansql = sql; int _startindex = -1;
            char[] _chars = sql.ToCharArray();

            for (int i = 0; i <= (_chars.Length - 1); i++)
            {
                if (_chars[i].ToString().ToUpper().In("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
                                                      "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", 
                                                      "Y", "Z"))
                { _startindex = i; break; }
            }

            if (_startindex >= 0) _cleansql = sql.Substring(_startindex, sql.Length - _startindex);

            return _cleansql;
        }

        private List<string> GetSqlPatternList()
        {
            List<string> _patterns = new List<string>();

            // SELECT (DISTINCT)
            _patterns.Add("^(S|s)(E|e)(L|l)(E|e)(C|c)(T|t)[\\n\\r\\t ]+((D|d)(I|i)(S|s)(T|t)(I|i)(N|n)(C|c)(T|t))?");

            // INSERT INTO [tablename]
            _patterns.Add ("^(I|i)(N|n)(S|s)(E|e)(R|r)(T|t)[\\n\\r\\t ]+(I|i)(N|n)(T|t)(O|o)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // UPDATE [tablename] SET
            _patterns.Add("^(U|u)(P|p)(D|d)(A|a)(T|t)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+[\\n\\r\\t ]+(S|s)(E|e)(T|t)[\\n\\r\\t ]+");

            // DELETE FROM [tablename]
            _patterns.Add ("^(D|d)(E|e)(L|l)(E|e)(T|t)(E|e)[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // ALTER (TABLE) (COLUMN) (VIEW) (PROCEDURE) (DATABASE) [name]
            _patterns.Add("^(A|a)(L|l)(T|t)(E|e)(R|r)[\\n\\r\\t ]+((T|t)(A|a)(B|b)(L|l)(E|e))?((C|c)(O|o)(L|l)(U|u)(M|m)(N|n))?((V|v)(I|i)(E|e)(W|w))?((F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n))?" +
                          "((P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e))?((D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e))?[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // DROP [(TABLE) (COLUMN) (VIEW) (PROCEDURE) (DATABASE) (TRIGGER) (FOREIGN KEY) (PRIMARY KEY) (INDEX)] (IF EXISTS)
            _patterns.Add ("^(D|d)(R|r)(O|o)(P|p)[\\n\\r\\t ]+((T|t)(A|a)(B|b)(L|l)(E|e))?((C|c)(O|o)(L|l)(U|u)(M|m)(N|n))?((V|v)(I|i)(E|e)(W|w))?((F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n))?" +
                           "((P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e))?((D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e))?((T|t)(R|r)(I|i)(G|g)(G|g)(E|e)(R|r))?" +
                           "((F|f)(O|o)(R|r)(E|e)(I|i)(G|g)(N|n)[\\n\\r\\t ]+(K|k)(E|e)(Y|y))?((P|p)(R|r)(I|i)(M|m)(A|a)(R|r)(Y|y)[\\n\\r\\t ]+(K|k)(E|e)(Y|y))?((I|i)(N|n)(D|d)(E|e)(X|x))?[\\n\\r\\t ]+" +
                           "((I|i)(F|f)[\\n\\r\\t ]+(E|e)(X|x)(I|i)(S|s)(T|t)(S|s)[\\n\\r\\t ]+)?[a-zA-Z0-9`\\[\\]_\\.]+");

            // CREATE (TABLE) (COLUMN) (VIEW) (PROCEDURE) (DATABASE) (TRIGGER) (INDEX) [name]
            _patterns.Add("^(C|c)(R|r)(E|e)(A|a)(T|t)(E|e)[\\n\\r\\t ]+((T|t)(A|a)(B|b)(L|l)(E|e))?((C|c)(O|o)(L|l)(U|u)(M|m)(N|n))?((V|v)(I|i)(E|e)(W|w))?((F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n))?" +
                          "((P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e))?((D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e))?((T|t)(R|r)(I|i)(G|g)(G|g)(E|e)(R|r))?((I|i)(N|n)(D|d)(E|e)(X|x))?[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SET (GLOBAL) (SESSION) [variable]=
            _patterns.Add("^(S|s)(E|e)(T|t)[\\n\\r\\t ]+((G|g)(L|l)(O|o)(B|b)(A|a)(L|l)[\\n\\r\\t ]+)?((S|s)(E|e)(S|s)(S|s)(I|i)(O|o)(N|n)[\\n\\r\\t ]+)?[a-zA-Z0-9_`@]+[\\n\\r\\t =]+");

            // TRUNCATE TABLE [tablename]
            _patterns.Add ("^(T|t)(R|r)(U|u)(N|n)(C|c)(A|a)(T|t)(E|e)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // CALL [procedurename]
            _patterns.Add ("^(C|c)(A|a)(L|l)(L|l)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.\\(\\)]+");

            // ANALYZE (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(A|a)(N|n)(A|a)(L|l)(Y|y)(Z|z)(E|e)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // CHECK TABLE
            _patterns.Add("^(C|c)(H|h)(E|e)(C|c)(K|k)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)");

            // OPTIMIZE (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(O|o)(P|p)(T|t)(I|i)(M|m)(I|i)(Z|z)(E|e)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // REPAIR (LOCAL) (NO_WRITE_BIN_LOG) TABLE
            _patterns.Add("^(R|r)(E|e)(P|p)(A|a)(I|i)(R|r)[\\r\\n\\t ]+((L|l)(O|o)(C|c)(A|a)(L|l)[\\n\\r\\t ]+)?((N|n)(O|o)_(W|w)(R|r)(I|i)(T|t)(E|e)_(T|t)(O|o)_(B|b)(I|i)(N|n)(L|l)(O|o)(G|g)[\\n\\r\\t ]+)?(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+");

            // DESCRIBE [tablename]
            _patterns.Add ("^(D|d)(E|e)(S|s)(C|c)(R|r)(I|i)(B|b)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW (FULL) COLUMNS FROM [name]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((F|f)(U|u)(L|l)(L|l)[\\n\\r\\t ]+)?(C|c)(O|o)(L|l)(U|u)(M|n)(S|s)[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW CREATES (DATABASE) (FUNCTION) (PROCEDURE) (TABLE) [name]
            _patterns.Add ("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(C|c)(R|r)(E|e)(A|a)(T|t)(E|e)[\\n\\r\\t ]+((D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e)[\\n\\r\\t ]+)?((F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+)?" +
                           "((P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+)?((T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+)?[a-zA-Z0-9`\\[\\]_\\.\\\\]+");

            // SHOW DATABASES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(D|d)(A|a)(T|t)(A|a)(B|b)(A|a)(S|s)(E|e)(S|s)[\\n\\r\\t ]+");

            // SHOW ENGINE [name]
            _patterns.Add ("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(E|e)(N|n)(G|g)(I|i)(N|n)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.-]+");

            // SHOW (STORAGE) ENGINES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((S|s)(T|t)(O|o)(R|r)(A|a)(G|g)(E|e)[\\n\\r\\t ]+)?(E|e)(N|n)(G|g)(I|i)(N|n)(E|e)(S|s)");

            // SHOW ERRORS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(E|e)(R|r)(R|r)(O|o)(R|r)(S|s)");

            // SHOW FUNCTION CODE [functionname]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+(C|c)(O|o)(D|d)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW FUNCTION STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(F|f)(U|u)(N|n)(C|c)(T|t)(I|i)(O|o)(N|n)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW INDEX FROM [name]
            _patterns.Add ("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(I|i)(N|n)(D|d)(E|e)(X|x)[\\n\\r\\t ]+(F|f)(R|r)(O|o)(M|m)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW INNODB STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(I|i)(N|n)(N|n)(O|o)(D|d)(B|b)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW PROCEDURE CODE [procedurename]
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+(C|c)(O|o)(D|d)(E|e)[\\n\\r\\t ]+[a-zA-Z0-9`\\[\\]_\\.]+");

            // SHOW PROCEDURE STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(O|o)(C|c)(E|e)(D|d)(U|u)(R|r)(E|e)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW OPEN TABLES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(O|o)(P|p)(E|e)(N|n)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)(S|s)");

            // SHOW PRIVILEGES
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(P|p)(R|r)(I|i)(V|v)(I|i)(L|l)(E|e)(G|g)(E|e)(S|s)");

            // SHOW PROCESSLIST
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((F|f)(U|u)(L|l)(L|l)[\\n\\r\\t ]+)?(P|p)(R|r)(O|o)(C|c)(E|e)(S|s)(S|s)(L|l)(I|i)(S|s)(T|t)");

            // SHOW (PROFILE) (PROFILES)
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((P|p)(R|r)(O|o)(F|f)(I|i)(L|l)(E|e)(S|s)|(P|p)(R|r)(O|o)(F|f)(I|i)(L|l)(E|e))");

            // SHOW (GLOBAL) (SESSION) (STATUS) (VARIABLES) 
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+((G|g)(L|l)(O|o)(B|b)(A|a)(L|l)[\\n\\r\\t ]+)?((S|s)(E|e)(S|s)(S|s)(I|i)(O|o)(N|n)[\\n\\r\\t ]+)?((S|s)(T|t)(A|a)(T|t)(U|u)(S|s)|(V|v)(A|a)(R|r)(I|i)(A|a)(B|b)(L|l)(E|e)(S|s))");

            // SHOW TABLE STATUS
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(T|t)(A|a)(B|b)(L|l)(E|e)[\\n\\r\\t ]+(S|s)(T|t)(A|a)(T|t)(U|u)(S|s)");

            // SHOW (TABLES) (TRIGGERS) (WARNINGS)
            _patterns.Add("^(S|s)(H|h)(O|o)(W|w)[\\n\\r\\t ]+(((T|t)(A|a)(B|b)(L|l)(E|e)(S|s)|(T|t)(R|r)(I|i)(G|g)(G|g)(E|e)(R|r)(S|s))|(W|w)(A|a)(R|r)(N|n)(I|i)(N|n)(G|g)(S|s))");

            return _patterns;
        }

        private bool IsRevalidated(string value)
        {
            bool _isvalid = true;

            if (!String.IsNullOrEmpty(value.RLTrim()))
            {
                int _startindex = 0;
                char[] _chars = value.ToCharArray();

                for (int i = 0; i <= (_chars.Length - 1); i++)
                {
                    if (_chars[i].ToString().ToUpper().In("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
                                                          "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
                                                          "Y", "Z"))
                    { _startindex = i; break; }
                }

                if (_startindex >= 0)
                {
                    List<string> _patterns = GetSqlPatternList();
                    string _reparsedvalue = value.Substring(_startindex, value.Length - _startindex);

                    foreach (string pattern in _patterns)
                    {
                        _isvalid = Regex.IsMatch(_reparsedvalue, pattern);

                        if (_isvalid)
                        {
                            string _incompletestringblock = "'[\\n\\r\\t a-zA-Z0-9~`!@#$\\%\\^&\\*\\(\\)-_\\+=\\{\\}\\[\\]\\\\/\\|:;\"<>,\\.\\?']+;";
                            MatchCollection _matches = Regex.Matches(_reparsedvalue, _incompletestringblock);
                            if (_matches.Count > 0)
                            {
                                _isvalid = !_reparsedvalue.EndsWith(_matches[0].Value);
                                if (!_isvalid)
                                {
                                    MatchCollection _clips = Regex.Matches(_reparsedvalue, "'");
                                    _isvalid = VisualBasic.CBool(_clips.Count % 2 == 0);
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return _isvalid;
        }

        private bool IsValidSqlStatement(string value)
        {
            bool _isvalid = false;
            string _parsedvalue = value.RLTrim();
            List<string> _patterns = GetSqlPatternList();

            foreach (string pattern in _patterns)
            {
                _isvalid = _isvalid || Regex.IsMatch(_parsedvalue, pattern);
                if (_isvalid)
                {
                    string _incompletestringblock = "'[\\n\\r\\t a-zA-Z0-9~`!@#$\\%\\^&\\*\\(\\)-_\\+=\\{\\}\\[\\]\\\\/\\|:;\"<>,\\.\\?']+;";
                 
                    MatchCollection _matches = Regex.Matches(_parsedvalue, _incompletestringblock);
                    if (_matches.Count > 0)
                    {
                        _isvalid = !_parsedvalue.EndsWith(_matches[0].Value);
                        if (!_isvalid)
                        {
                            MatchCollection _clips = Regex.Matches(_parsedvalue, "'");
                            _isvalid = VisualBasic.CBool(_clips.Count % 2 == 0);
                        }
                    }
                }
            }

            if (!_isvalid)
            {
                int _startindex = 0;
                char[] _chars = value.ToCharArray();

                for (int i = 0; i <= (_chars.Length - 1); i++)
                {
                    if (_chars[i].ToString().ToUpper().In("A", "B", "C", "E", "F", "G", "H", "I", "J", "K", "L",
                                                          "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W",
                                                          "X", "Y", "Z"))
                    { _startindex = i; break; }
                }

                if (_startindex >= 0)
                {
                    string _reparsedvalue = value.Substring(_startindex, value.Length - _startindex);
                    
                    foreach (string pattern in _patterns)
                    {
                        string _incompletestringblock = "'[\\n\\r\\t a-zA-Z0-9~`!@#$\\%\\^&\\*\\(\\)-_\\+=\\{\\}\\[\\]\\\\/\\|:;\"<>,\\.\\?']+;";
                        MatchCollection _matches = Regex.Matches(_reparsedvalue, _incompletestringblock);
                        if (_matches.Count > 0)
                        {
                            _isvalid = !_reparsedvalue.EndsWith(_matches[0].Value);
                            if (!_isvalid)
                            {
                                MatchCollection _clips = Regex.Matches(_reparsedvalue, "'");
                                _isvalid = VisualBasic.CBool(_clips.Count % 2 == 0);
                            }
                        }

                        break;
                    }
                }
            }

            return _isvalid;
        }

        private void Parse()
        {
            CommandStatements.Clear();

            if (_que != null)
            {
                string _statement = _que.CommandText; string _delimiter = Development.Materia.Database.Que.Delimiter;
                string[] _rawstatements = _statement.Split(_delimiter.ToCharArray());

                List<string> _statements = new List<string>(); int _laststatement = -1;
                bool _unclosedblock = false; _delimiter = ";";
                string _closingblockpattern ="^([\\n\\r\\t a-zA-Z0-9~`!@#$\\%\\^&\\*\\(\\)-_\\+=\\{\\}\\[\\]\\\\/\\|:;\"<>,\\.\\?']+)?'(([\\n\\r\\t ,\\)]+|([\\n\\r\\t ]+(A|a)(S|s))?|[\\n\\r\\t ,]+))?";

                for (int i = 0; i <= (_rawstatements.Length - 1); i++)
                {
                    string s = _rawstatements[i]; string _sql = CleanStatement(s);
                    if (!String.IsNullOrEmpty(_sql.RLTrim()))
                    {
                        _sql += _delimiter;
                        if (IsValidSqlStatement(_sql))
                        {
                            if (IsRevalidated(_sql))
                            {
                                if (!_unclosedblock) _laststatement = _commandstatements.Add(_sql);
                                else
                                {
                                    if (_laststatement > -1)
                                    {
                                        _commandstatements[_laststatement] += s + _delimiter;
                                        MatchCollection _matches = Regex.Matches(s, _closingblockpattern);
                                        foreach (Match m in _matches)
                                        {
                                            _unclosedblock = s.StartsWith(m.Value);
                                            if (_unclosedblock)
                                            {
                                                MatchCollection _quotes = Regex.Matches(_commandstatements[_laststatement], "'");
                                                _unclosedblock = !((_quotes.Count > 0) && (_quotes.Count % 2 == 0)); break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (_laststatement > -1)
                                {
                                    if (_unclosedblock)
                                    {
                                        _commandstatements[_laststatement] += s + _delimiter;
                                        MatchCollection _matches = Regex.Matches(s, _closingblockpattern);
                                        foreach (Match m in _matches)
                                        {
                                            _unclosedblock = s.StartsWith(m.Value);
                                            if (_unclosedblock)
                                            {
                                                MatchCollection _quotes = Regex.Matches(_commandstatements[_laststatement], "'");
                                                _unclosedblock = !((_quotes.Count > 0) && (_quotes.Count % 2 == 0)); break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _laststatement = _commandstatements.Add(_sql); _unclosedblock = true;
                                    }
                                }
                                else
                                {
                                    _laststatement = _commandstatements.Add(_sql); _unclosedblock = true;
                                }
                            }
                        }
                        else
                        {
                            if (_laststatement > -1 && _unclosedblock)
                            {
                                _commandstatements[_laststatement] += s + _delimiter;
                                MatchCollection _matches = Regex.Matches(s, _closingblockpattern);
                                foreach (Match m in _matches)
                                {
                                    _unclosedblock = s.StartsWith(m.Value);
                                    if (_unclosedblock)
                                    {
                                        MatchCollection _quotes = Regex.Matches(_commandstatements[_laststatement], "'");
                                        _unclosedblock = !((_quotes.Count > 0) && (_quotes.Count % 2 == 0)); break;
                                    }
                                }
                            }
                            else
                            {
                                _laststatement = _commandstatements.Add(_sql); _unclosedblock = true;
                            }
                        }
                    }
                    else
                    {
                        if (i < (_rawstatements.Length - 1))
                        {
                            if (String.IsNullOrEmpty(_sql.RLTrim()))
                            {
                                if (_laststatement > -1)
                                {
                                    _commandstatements[_laststatement] += s + _delimiter;
                                    MatchCollection _quotes = Regex.Matches(_commandstatements[_laststatement], "'");
                                    _unclosedblock = !((_quotes.Count > 0) && (_quotes.Count % 2 == 0));
                                }
                            }
                        }
                    }
                }
            }

            Materia.RefreshAndManageCurrentProcess();
        }

        #endregion

    }

    /// <summary>
    /// Collection of database command statements.
    /// </summary>
    public class CommandStatementCollection : CollectionBase
    {

        #region "constructors"
        
        /// <summary>
        /// Creates a new instance of CommandStatementCollection.
        /// </summary>
        /// <param name="parser"></param>
        public CommandStatementCollection(CommandParser parser)
        { _parser = parser; }

        #endregion

        #region "properties"

        /// <summary>
        /// Gets or sets the command statement at the specified index of the collection.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get { return List[index].ToString(); }
            set { List[index] = value; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CommandParser _parser = null;

        /// <summary>
        /// Gets the CommandParser object associated with the collection class.
        /// </summary>
        public CommandParser Parser
        {
            get { return _parser; }
        }

        #endregion

        #region "methods"

        /// <summary>
        /// Adds a new command statement into the collection.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Add(string sql)
        { return List.Add(sql); }

        /// <summary>
        ///  Returns whether the specified sql command statement already exists in the collection or not.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool Contains(string sql)
        { return List.Contains(sql); }

        /// <summary>
        /// Removes the specified command statement from the collection.
        /// </summary>
        /// <param name="sql"></param>
        public void Remove(string sql)
        { List.Remove(sql); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _tostring = "";

            foreach (string sql in List)
            { _tostring += (String.IsNullOrEmpty(_tostring.RLTrim()) ? "" : "\n") + "[" + sql + "]"; }

            return _tostring;
        }

        #endregion

    }

}
