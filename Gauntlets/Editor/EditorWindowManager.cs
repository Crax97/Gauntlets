using Eto;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gauntlets.Editor
{
    class EditorWindowManager : Application
    {

        EntityEditor editor = new EntityEditor();

        public EditorWindowManager(Platform platform) : base(platform)
        {
            new Thread(new ThreadStart(() =>
            {

                Run(editor);
            }));
            editor.Show();
        }

        public EntityEditor GetEntityEditor()
        {
            return editor;
        }

    }
}
