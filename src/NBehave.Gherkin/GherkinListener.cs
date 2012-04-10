using System;
using System.Collections.Generic;
using System.Linq;
using GurkBurk;

namespace NBehave.Gherkin
{
    public class GherkinListener : Listener
    {
        private readonly IListener listener;
        private Action tableAction = () => { };
        private IList<IList<Token>> table = new List<IList<Token>>();

        public GherkinListener(IListener listener)
        {
            this.listener = listener;
        }

        public void docString(string str, int line)
        {
            tableAction();
            var position = new LineInFile(line);
            listener.DocString(new Token(str.Trim(), position));
        }

        public void feature(string feature, string title, string narrative, int line)
        {
            tableAction();
            var lineInFile = new LineInFile(line);
            if (Environment.NewLine == "\r\n")
                narrative = narrative.Replace("\r", "").Replace("\n", Environment.NewLine);
            listener.Feature(new Token(feature.Trim(), lineInFile), new Token(title, lineInFile), new Token(narrative, lineInFile));
        }

        public void background(string background, string title, string str3, int line)
        {
            var lineInFile = new LineInFile(line);
            listener.Background(new Token(background.Trim(), lineInFile), new Token(title, lineInFile));
        }

        public void scenario(string scenario, string title, string str3, int line)
        {
            tableAction();
            var lineInFile = new LineInFile(line);
            listener.Scenario(new Token(scenario.Trim(), lineInFile), new Token(title, lineInFile));
        }

        public void scenarioOutline(string outline, string title, string str3, int line)
        {
            scenario(outline.Trim(), title, str3, line);
        }

        public void examples(string examples, string str2, string str3, int line)
        {
            tableAction();
            table = new List<IList<Token>>();
            var position = new LineInFile(line);
            listener.Examples(new Token(examples.Trim(), position), new Token(str2, position));
        }

        public void step(string step, string stepText, int line)
        {
            tableAction();
            var position = new LineInFile(line);
            listener.Step(new Token(step.Trim(), position), new Token(stepText, position));
        }

        public void comment(string str, int line)
        {
            listener.Comment(new Token(str, new LineInFile(line)));
        }

        public void tag(string str, int line)
        {
            listener.Tag(new Token(str, new LineInFile(line)));
        }

        public void row(List<string> l, int line)
        {
            if (table.Any() == false)
                tableAction = () => ParsedTable(new LineInFile(line));

            var columns = l.ToArray();
            var lineInFile = new LineInFile(line);
            var cols = columns.Select(_ => new Token(_, lineInFile)).ToList();
            table.Add(cols);
        }

        public void eof()
        {
            tableAction.Invoke();
            listener.Eof();
        }

        private void ParsedTable(LineInFile position)
        {
            listener.Table(table, position);
            tableAction = () => { };
            table = new List<IList<Token>>();
        }
    }
}