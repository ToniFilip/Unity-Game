// using System;
// using UnityEngine;
// using UnityEngine.UIElements;

// namespace HudElements
// {
//     public class BMTemplate : VisualElement
//     {
//         [UnityEngine.Scripting.Preserve]
//         public new class UxmlFactory : UxmlFactory<ElementBM> { }

//         public BMTemplate()
//         {
//             VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/BMTemplate");
//             VisualElement bmElement = vt.Instantiate();
//         }
//     }
// }

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace PopupTest
{
    public class BMTemplate : VisualElement
    {
        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<BMTemplate> { }

        public BMTemplate()
        {
            VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/BMTemplate");
            VisualElement bmElement = vt.Instantiate();
        }

        public event Action confirmed;
        public event Action cancelled;

        private void OnConfirm()
        {
            confirmed?.Invoke();
        }

        private void OnCancel()
        {
            cancelled?.Invoke();
        }
    }
}
