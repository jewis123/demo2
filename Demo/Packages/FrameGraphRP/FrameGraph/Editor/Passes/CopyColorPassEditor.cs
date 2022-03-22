using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace FrameGraph
{
    [CustomPropertyDrawer(typeof(CopyColorPassWrapper.Settings))]
    public class CopyColorPassEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var sampleMode = property.FindPropertyRelative("sampleMode");
            var samplingMaterial = property.FindPropertyRelative("samplingMaterial");
            var copyMaterial = property.FindPropertyRelative("copyMaterial");
            var field = new PropertyField(sampleMode);
            container.Add(field);
            field = new PropertyField(samplingMaterial);
            container.Add(field);
            field = new PropertyField(copyMaterial);
            container.Add(field);

            return container;
        }
    }
}