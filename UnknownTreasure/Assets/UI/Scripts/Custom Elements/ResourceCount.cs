using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HudElements
{
    public class ResourceCount : VisualElement, INotifyValueChanged<int>
    {

        // public int width { get; set; }
        // public int height { get; set; }

        public void SetValueWithoutNotify(int newValue)
        {
            m_value = newValue;
        }

        private int m_value;

        public int value
        {
            get
            {
                m_value = Mathf.Clamp(m_value, 0, 9999);
                return m_value;
            }
            set
            {
                if (EqualityComparer<int>.Default.Equals(m_value, value))
                {
                    return;
                }
                if (this.panel != null)
                {
                    using (ChangeEvent<int> pooled = ChangeEvent<int>.GetPooled(m_value, value))
                    {
                        pooled.target = (IEventHandler)this;
                        this.SetValueWithoutNotify(value);
                        this.SendEvent((EventBase)pooled);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }

        /*public Color imgColor { get; set; }*/

        private VisualElement resParent;
        private Label resLabel;
        /*private VisualElement resImg;*/

        public new class UxmlFactory : UxmlFactory<ResourceCount, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            // UxmlIntAttributeDescription m_width = new UxmlIntAttributeDescription() { name = "width", defaultValue = 300 };
            // UxmlIntAttributeDescription m_height = new UxmlIntAttributeDescription() { name = "height", defaultValue = 50 };
            UxmlIntAttributeDescription m_value = new UxmlIntAttributeDescription() { name = "value", defaultValue = 0 };
            /*UxmlColorAttributeDescription m_imgColor = new UxmlColorAttributeDescription() { name = "img-color", defaultValue = Color.red };*/

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as ResourceCount;
                // ate.width = m_width.GetValueFromBag(bag, cc);
                // ate.height = m_height.GetValueFromBag(bag, cc);
                ate.value = m_value.GetValueFromBag(bag, cc);
                /*ate.imgColor = m_imgColor.GetValueFromBag(bag, cc);*/

                ate.Clear();
                VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/ResourceCount");
                VisualElement resourceCount = vt.Instantiate();
                ate.resParent = resourceCount.Q<VisualElement>("resourcecount");
                ate.resLabel = resourceCount.Q<Label>("label");
                /*ate.resImg = resourceCount.Q<VisualElement>("img");*/
                ate.Add(resourceCount);

                // ate.resParent.style.width = ate.width;
                // ate.resParent.style.height = ate.height;
                // ate.resImg.style.width = ate.height;
                // ate.style.width = ate.width;
                // ate.style.height = ate.height;
                /*ate.resImg.style.backgroundColor = ate.imgColor;*/

                ate.RegisterValueChangedCallback(ate.UpdateResourceCount);
                ate.AdjustCount();
            }
        }

        public void UpdateResourceCount(ChangeEvent<int> evt)
        {
            AdjustCount();
        }

        private void AdjustCount()
        {
            resLabel.text = value.ToString("D" + 4);
        }
    }
}
