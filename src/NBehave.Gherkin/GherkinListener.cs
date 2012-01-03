using System;
using System.Collections.Generic;
using System.Linq;
using gherkin.lexer;
using java.util;

namespace NBehave.Gherkin
{
    public class GherkinListener : Listener
    {
        private readonly IListener _listener;
        private Action _tableAction = () => { };
        private IList<IList<Token>> _table = new List<IList<Token>>();

        public GherkinListener(IListener listener)
        {
            _listener = listener;
        }

        public void docString(string str, int line)
        { }

        public void feature(string feature, string title, string narrative, int line)
        {
            _tableAction();
            var lineInFile = new LineInFile(line);
            if (Environment.NewLine == "\r\n")
                narrative = narrative.Replace("\r", "").Replace("\n", Environment.NewLine);
            _listener.Feature(new Token(feature, lineInFile), new Token(title, lineInFile), new Token(narrative, lineInFile));
        }

        public void background(string background, string title, string str3, int line)
        {
            var lineInFile = new LineInFile(line);
            _listener.Background(new Token(background, lineInFile), new Token(title, lineInFile));
        }

        public void scenario(string scenario, string title, string str3, int line)
        {
            _tableAction();
            var lineInFile = new LineInFile(line);
            _listener.Scenario(new Token(scenario, lineInFile), new Token(title, lineInFile));
        }

        public void scenarioOutline(string outline, string title, string str3, int line)
        {
            scenario(outline, title, str3, line);
        }

        public void examples(string examples, string str2, string str3, int line)
        {
            _tableAction();
            _table = new List<IList<Token>>();
            var position = new LineInFile(line);
            _listener.Examples(new Token(examples, position), new Token(str2, position));
        }

        public void step(string step, string stepText, int line)
        {
            _tableAction();
            var position = new LineInFile(line);
            _listener.Step(new Token(step, position), new Token(stepText, position));
        }

        public void comment(string str, int line)
        {
            _listener.Comment(new Token(str, new LineInFile(line)));
        }

        public void tag(string str, int line)
        {
            _listener.Tag(new Token(str, new LineInFile(line)));
        }

        public void row(List l, int line)
        {
            if (_table.Any() == false)
                _tableAction = () => ParsedTable(new LineInFile(line));

            var columns = l.toArray();
            var lineInFile = new LineInFile(line);
            var cols = columns.Select(_ => new Token(_.ToString(), lineInFile)).ToList();
            _table.Add(cols);
        }

        public void eof()
        {
            _tableAction.Invoke();
            _listener.Eof();
        }

        private void ParsedTable(LineInFile position)
        {
            _listener.Table(_table, position);
            _tableAction = () => { };
            _table = new List<IList<Token>>();
        }
    }
}