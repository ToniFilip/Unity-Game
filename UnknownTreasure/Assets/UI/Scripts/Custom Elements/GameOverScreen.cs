using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIMenues
{
    public class GameOverScreen : VisualElement, INotifyValueChanged<bool>
    {

        public void SetValueWithoutNotify(bool newValue)
        {
            m_value = newValue;
        }

        private bool m_value;

        public bool value
        { 
            get 
            {
                return m_value;
            }
            set
            {
                if(EqualityComparer<bool>.Default.Equals(m_value, value))
                {
                    return;
                }
                if(this.panel != null)
                {
                    using (ChangeEvent<bool> pooled = ChangeEvent<bool>.GetPooled(m_value, value))
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

        private Label gameOverLabel;
        private Button replayButton;
        private Button menuButton;

        public new class UxmlFactory: UxmlFactory<GameOverScreen, UxmlTraits>{ }

        public new class UxmlTraits: VisualElement.UxmlTraits
        {
            UxmlBoolAttributeDescription m_value = new UxmlBoolAttributeDescription(){name = "value", defaultValue = true};

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as GameOverScreen;
                ate.value = m_value.GetValueFromBag(bag, cc);

                ate.Clear();
                VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/GameOverScreen");
                VisualElement GameOverScreen = vt.Instantiate();
                // ate.gameOverLabel = GameOverScreen.Q<Label>("gameOverLabel");
                // ate.replayButton = GameOverScreen.Q<Button>("replay");
                // ate.menuButton = GameOverScreen.Q<Button>("menu");

                // ate.replayButton.clicked += () => Debug.Log("replay clicked");
                // ate.replayButton.text = "Replay Level";
                // ate.menuButton.clicked += () => Debug.Log("back to menu clicked");
                // ate.menuButton.text = "Back To Menu";

                ate.Add(GameOverScreen);
                ate.RegisterValueChangedCallback(ate.UpdateLayout);
            }
        }

        public void UpdateLayout(ChangeEvent<bool> evt)
        {
            // if(value)
            // {
            //     gameOverLabel.text = "Congratulations!";
            // }
            // else
            // {
            //     gameOverLabel.text = "Game over!";
            // }
        }
    }
}
