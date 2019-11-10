using System;
using System.ComponentModel;
using System.Drawing;

namespace MioMatrix
{
    public class MatrixButton : System.Windows.Forms.Button
    {
        private static object _lockObject = "";
        private bool _active;
        public int DestinationPort { get; set; }

        public MatrixButton()
        {
            _active = false;
            BackColor = SystemColors.Control;
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                SetColor();
            }
        }

        private void SetColor()
        {
            this.BackColor = _active ? Color.Red : SystemColors.Control;
        }

        protected override void OnClick(EventArgs e)
        {
            lock (_lockObject)
            {
                BackColor = Color.DarkOrange;

                _active = !_active;

                var rowPanel = Parent as MatrixRowPanel;

                if (Active)
                {
                    rowPanel.ActiveDestinations.Add(DestinationPort);
                }
                else
                {
                    rowPanel.ActiveDestinations.Remove(DestinationPort);
                }

                QueryManager.Instance.SendMidiPortRoute(rowPanel.SourcePort, rowPanel.ActiveDestinations);

                SetColor();
            }

            base.OnClick(e);
        }
    }
}