using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HudElements
{
    public class BMElement : VisualElement, INotifyValueChanged<(bool, string)>
    {

        // public int width { get; set; }
        // public int height { get; set; }

        public void SetValueWithoutNotify((bool, string) newValue)
        {
            m_value = newValue;
        }

        private (bool, string) m_value;

        public (bool, string) value
        { 
            get 
            {
                return m_value;
            }
            set
            {
                if(EqualityComparer<(bool, string)>.Default.Equals(m_value, value))
                {
                    return;
                }
                if(this.panel != null)
                {
                    using (ChangeEvent<(bool, string)> pooled = ChangeEvent<(bool, string)>.GetPooled(m_value, value))
                    {
                        pooled.target = (IEventHandler) this;
                        this.SetValueWithoutNotify(value);
                        this.SendEvent((EventBase) pooled);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }

        private bool m_selected;
        public bool selected
        { 
            get 
            {
                return m_value.Item1;
            }
            set
            {
                m_selected = value;
                this.SetValueWithoutNotify((value, m_value.Item2));
            }
        }

        private string m_imageSource;
        public string imageSource
        { 
            get 
            {
                return m_value.Item2;
            }
            set
            {
                m_imageSource = value;
                this.SetValueWithoutNotify((m_value.Item1, value));
            }
        }
        

        // public Color fillColor { get; set; }
        // public Color fillColorSelected { get; set; }

        private VisualElement bmElementParent;
        private VisualElement bmElementBackground;

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory: UxmlFactory<BMElement, UxmlTraits>{ }


        public new class UxmlTraits: VisualElement.UxmlTraits
        {
            // UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription(){name = "width", defaultValue = 150};
            // UxmlIntAttributeDescription m_height = new UxmlIntAttributeDescription(){name = "height", defaultValue = 150};
            UxmlBoolAttributeDescription m_selected = new UxmlBoolAttributeDescription(){name = "selected", defaultValue = false};
            // UxmlColorAttributeDescription m_fillColor = new UxmlColorAttributeDescription(){name = "fill-color", defaultValue = Color.red};
            // UxmlColorAttributeDescription m_fillColorSelected = new UxmlColorAttributeDescription(){name = "fill-color-selected", defaultValue = Color.red};
            UxmlStringAttributeDescription m_imageSource = new UxmlStringAttributeDescription(){name = "image-source", defaultValue = null};

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as BMElement;
                // ate.width = m_width.GetValueFromBag(bag, cc);
                // ate.height = m_height.GetValueFromBag(bag, cc);
                ate.selected = m_selected.GetValueFromBag(bag, cc);
                // ate.fillColor = m_fillColor.GetValueFromBag(bag, cc);
                // ate.fillColorSelected = m_fillColorSelected.GetValueFromBag(bag, cc);
                ate.imageSource = m_imageSource.GetValueFromBag(bag, cc);

                ate.Clear();
                VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/BMElement");
                VisualElement bmElement = vt.Instantiate();
                ate.bmElementParent = bmElement.Q<VisualElement>("bmelement");
                ate.bmElementBackground = bmElement.Q<VisualElement>("background");

                // ate.bmElementParent.style.width = ate.width;
                // ate.bmElementParent.style.height = ate.height;
                // ate.style.width = ate.width;
                // ate.style.height = ate.height;
                // ate.bmElementBackground.style.backgroundColor = ate.fillColor;

                ate.Add(bmElement);

                Texture2D myTexture = Resources.Load(ate.imageSource) as Texture2D;
                ate.bmElementBackground.style.backgroundImage = myTexture;
                ate.RegisterValueChangedCallback(ate.UpdateBMElement);
                ate.UpdateSelected();
            }
        }

        public void UpdateBMElement(ChangeEvent<(bool, string)> evt)
        {
            UpdateSelected();
            UpdateImage();
        }

        private void UpdateSelected()
        {
            if(value.Item1)
            {
                // bmElementBackground.style.backgroundColor = fillColorSelected;
            }
            else{
                // bmElementBackground.style.backgroundColor = fillColor;
            }
        }

        private void UpdateImage()
        {
            Texture2D myTexture = Resources.Load(imageSource) as Texture2D;
            bmElementBackground.style.backgroundImage = myTexture;
        }
    }
}
