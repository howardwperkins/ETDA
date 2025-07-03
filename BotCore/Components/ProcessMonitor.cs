using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace BotCore.Components
{
    public class ProcessMonitor : UpdateableComponent
    {
        public List<int> Processes = new List<int>();
        public event EventHandler Updated = delegate { };
        public event EventHandler Attached = delegate { };
        public event EventHandler Removed = delegate { };

        public ProcessMonitor()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(500.0));
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Timer.Reset();
                base.Pulse();

                var count = Process.GetProcessesByName("Darkages");
                Console.WriteLine("Process Count: " + count.Length);
                if (count.Length != Processes.Count)
                {
                    Console.WriteLine("Process Count Changed: " + count.Length + " - " + Processes.Count);
                    var id = count.Select(i => i.Id).Except(Processes).FirstOrDefault();
                    var p = count.FirstOrDefault(i => i.Id == id);

                    SetupProcess(p);
                }

                Updated(this, new EventArgs());
            }
        }

        private void SetupProcess(Process p)
        {
            if (Processes.Contains(p.Id))
            {
                return;
            }
            
            try {
                
                p.EnableRaisingEvents = true;
                p.Exited += PExited;

                Processes.Add(p.Id);
                Attached(p.Id, new EventArgs());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }           
        }

        void PExited(object sender, EventArgs e)
        {
            var p = (Process)sender;

            if (Processes.Contains(p.Id))
            {
                Processes.Remove(p.Id);
                Removed(p.Id, new EventArgs());
            }
        }
    }
}
