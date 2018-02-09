using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraxAwesomeEngine.Core;
using Eto.Forms;
using Microsoft.Xna.Framework;

namespace Gauntlets.Editor
{
    class EntityEditor : Form
    {

        class PropertiesEditor : Panel
        {
            TableLayout properties = new TableLayout();
            private Entity editedEntity = null;

            class Property : Panel
            {
                private Label key;
                public TextBox value;
                public Property(String name) : base()
                {
                    key = new Label();
                    value = new TextBox();
                    key.Text = name;
                    this.Content = new TableLayout(new TableRow(
                        new TableCell(key),
                        new TableCell(value)
                    ));
                }
            }

            public PropertiesEditor()
            {
                this.Size = new Eto.Drawing.Size(250, 200);
                properties.Rows.Add(new Property("position"));
                properties.Rows.Add(new Property("scale"));
                properties.Rows.Add(new Property("rotation"));

                this.Content = properties;
                
            }

            public void SetProperties(Entity e)
            {
                editedEntity = e;

                Property propertyPosition = properties.Rows[0].Cells[0].Control as Property;
                propertyPosition.value.Text = e.Transform.Position.X + " " + e.Transform.Position.Y;
                Property propertyScale = properties.Rows[1].Cells[0].Control as Property;
                propertyScale.value.Text = e.Transform.LocalScale.X + " " + e.Transform.LocalScale.Y;
                Property propertyRotation = properties.Rows[2].Cells[0].Control as Property;
                propertyRotation.value.Text = "" + e.Transform.Rotation;
                this.Content = properties;
            }

            public void SaveEdit()
            {
                if(editedEntity != null)
                {
                    int id = editedEntity.Id;
                    Property propertyPosition = properties.Rows[0].Cells[0].Control as Property;
                    Property propertyScale = properties.Rows[1].Cells[0].Control as Property;
                    Property propertyRotation = properties.Rows[2].Cells[0].Control as Property;

                    string[] posValues = propertyPosition.value.Text.Split(' ');
                    string[] scaleValues = propertyScale.value.Text.Split(' ');

                    Entity.knownEntities[id].Transform.Position = new Vector2(float.Parse(posValues[0]), float.Parse(posValues[1]));
                    Entity.knownEntities[id].Transform.LocalScale = new Vector2(float.Parse(scaleValues[0]), float.Parse(scaleValues[1]));
                    Entity.knownEntities[id].Transform.Rotation = float.Parse(propertyRotation.value.Text);

                    propertyPosition.value.Text = "";
                    propertyScale.value.Text = "";
                    propertyRotation.value.Text = "";
                    editedEntity = null;

                }
            }

        }
        TableLayout layout = new TableLayout();
        PropertiesEditor propEditor = new PropertiesEditor();

        ListBox entityList = null;

        public EntityEditor()
        {
            this.Title = "Entity Editor";
            this.ClientSize = new Eto.Drawing.Size(250, 600);
            this.Resizable = true;


        }

        public void PopulateEntityList()
        {
            entityList = new ListBox();
            foreach (Entity e in Entity.knownEntities)
            {
                entityList.Items.Add(e.Name, "" + e.Id);
            }
            SetupLayout(entityList);

            entityList.SelectedIndexChanged += EntityList_SelectedIndexChanged; ;
        }

        private void EntityList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = int.Parse(entityList.SelectedKey);
            propEditor.SaveEdit();
            propEditor.SetProperties(Entity.knownEntities[selectedId]);
        }

        private void SetupLayout(ListBox entityList)
        {
            layout.Rows.Add(new TableRow(entityList));
            layout.Rows.Add(new Label { Text = "Properties: " });
            layout.Rows.Add(propEditor);
            this.Content = layout;
        }

        public void InstantiateSelectedEntity(Vector2 position)
        {
            try
            {

                int SelectedId = int.Parse(entityList.SelectedKey);
                Entity instance = Entity.Instantiate(SelectedId);
                instance.Transform.Position = position;
                World.Current.AddEntity(instance);
            }
            catch (ArgumentNullException ex)
            {
                CraxAwesomeEngine.Core.Debugging.Debug.Log("You didn't select any entity!");
            }
        }
    }
}
