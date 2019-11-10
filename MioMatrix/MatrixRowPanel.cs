using System.Collections.Generic;

namespace MioMatrix
{
    public class MatrixRowPanel : System.Windows.Forms.Panel
    {
        public int SourcePort { get; set; }
        public HashSet<int> ActiveDestinations { get; set; }

        public MatrixRowPanel()
        {
            
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Refresh();
        }

        public void Refresh()
        {
            var routing = QueryManager.Instance.GetMidiPortRoute(SourcePort);
            if (routing != null)
            {
                ActiveDestinations = new HashSet<int>(routing.DestinationPorts);

                foreach (var control in Controls)
                {
                    var button = control as MatrixButton;
                    if (button != null)
                    {
                        if (ActiveDestinations.Contains(button.DestinationPort))
                        {
                            button.Active = true;
                        }
                    }
                }
            }
        }
    }
}