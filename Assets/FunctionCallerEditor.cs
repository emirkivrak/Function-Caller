using System;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_EDITOR
public class FunctionCallerEditor : EditorWindow
{
    GameObject targetObj;
    Component targetComponent;
    Component[] components;
    int selectedComponentIndex;
    int selectedMethods;
    int selectedMethodOld;

    Object _objectParamter = null;

    private int _intParameter = 0;
    private bool _boolParameter = false;
    private Vector2 _vector2Parameter;
    private Vector3 _vector3Parameter;
    private float _floatParameter;
    private string _stringParameter;
    private Vector4 _vector4Field;
    private Quaternion _quaternionField;

    private object returns;
    private System.Type returnType;

    [MenuItem("Window/FunctionCaller")]
    public static void ShowWindow()
    {
        GetWindow<FunctionCallerEditor>("Function Caller");
    }

    public void OnGUI()
    {
        targetObj = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObj, typeof(GameObject), true);



        if (targetObj != null)
        {
            components = targetObj.GetComponents<Component>();
            string[] componentNames = new string[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                componentNames[i] = components[i].GetType().Name;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Component");
            selectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, componentNames);
            EditorGUILayout.EndHorizontal();

            targetComponent = components[selectedComponentIndex];



            MethodInfo[] methods = targetComponent.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            string[] methodsNames = new string[methods.Length];

            for (int i = 0; i < methods.Length; i++)
            {
                methodsNames[i] = methods[i].Name;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Method");
            selectedMethods = EditorGUILayout.Popup(selectedMethods, methodsNames);
            if (selectedMethods != selectedMethodOld)
            {
                returns = null;
                selectedMethodOld = selectedMethods;
                returnType = null;
                DrawOutputBox();
            }

            if (methods.Length == 0)
            {
                return;
            }

            try
            {
                returnType = methods[selectedMethods].ReturnType;
                EditorGUILayout.EndHorizontal();
            }
            catch (Exception exception)
            {
                Debug.Log("Unsupported return type.");
                throw;  
            }
            

    
            EditorGUILayout.LabelField("PARAMETERS", EditorStyles.boldLabel);

            //�DEV
            ParameterInfo[] infos = methods[selectedMethods].GetParameters();
            object[] Params = new object[infos.Length];

            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].ParameterType == typeof(int))
                {
                    _intParameter = EditorGUILayout.IntField(infos[i].Name, _intParameter);
                    Params[i] = _intParameter;
                }
                else if (infos[i].ParameterType == typeof(bool))
                {
                    _boolParameter = EditorGUILayout.Toggle(infos[i].Name, _boolParameter);
                    Params[i] = _boolParameter;
                }
                else if (infos[i].ParameterType == typeof(Vector2))
                {
                    _vector2Parameter = EditorGUILayout.Vector2Field(infos[i].Name, _vector2Parameter);
                    Params[i] = _vector2Parameter;
                }
                else if (infos[i].ParameterType == typeof(Vector3))
                {
                    _vector3Parameter = EditorGUILayout.Vector3Field(infos[i].Name, _vector3Parameter);
                    Params[i] = _vector3Parameter;
                }
                else if (infos[i].ParameterType == typeof(float))
                {
                    _floatParameter = EditorGUILayout.FloatField(infos[i].Name, _floatParameter);
                    Params[i] = _floatParameter;
                }
                else if (infos[i].ParameterType == typeof(string))
                {
                    _stringParameter = EditorGUILayout.TextField(infos[i].Name, _stringParameter);
                    Params[i] = _stringParameter;
                }
                else if (infos[i].ParameterType == typeof(Vector4))
                {
                    _vector4Field = EditorGUILayout.Vector4Field(infos[i].Name, _vector4Field);
                    Params[i] = _vector4Field;
                }
                else if (infos[i].ParameterType == typeof(Quaternion))
                {
                    _vector3Parameter = EditorGUILayout.Vector3Field(infos[i].Name, _vector3Parameter);
                    _quaternionField = Quaternion.Euler(_vector3Parameter.x, _vector3Parameter.y, _vector3Parameter.z);
                    Params[i] = _quaternionField;
                }
                else
                {
                    _objectParamter =
                        EditorGUILayout.ObjectField(infos[i].Name, _objectParamter, infos[i].ParameterType, true);
                    Params[i] = _objectParamter;
                }
            }

            //BUTTON
            if (GUILayout.Button("Execute Function"))
            {
                //var returns =   methods[selectedMethods].Invoke(components[selectedComponentIndex],new object[]{_quaternionField});
                returns = methods[selectedMethods].Invoke(components[selectedComponentIndex], ReturnParams(Params));
                returnType = null;
            }
            DrawOutputBox();
        }
    }

    public void DrawOutputBox()
    {
        if (returns == null)
        {
            if (returnType != null)
            {
                if (returnType == typeof(void))
                {
                    EditorGUILayout.HelpBox("A void function selected.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Given function will return a " + returnType.ToString(), MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select a Function for see Return values", MessageType.Info);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Returned -->  " + returns.ToString(), MessageType.Info);
        }
    }

    public object[] ReturnParams(object[] Parameters)
    {
        if (Parameters.Length == 0)
            return null;
        return Parameters;
    }
}


#endif