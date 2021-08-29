#if VRC_SDK_VRCSDK3
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

public class Yelby_Environment : EditorWindow
{
    /*Global Variables*/
    GameObject avatar;
    VRC.SDK3.Avatars.Components.VRCAvatarDescriptor SDK;

    /*Bool Options*/
    bool bBase;
    bool bAdditive;
    bool bGesture;
    bool bAction;
    bool bFX;

    /*Controllers*/
    AnimatorController cBase;
    AnimatorController cAdditive;
    AnimatorController cGesture;
    AnimatorController cAction;
    AnimatorController cFX;

    /*Scroll Views*/
    Vector2 scrollLocation;

    [MenuItem("Yelby/Yelby Environment")]
    public static void ShowWindow()
    {
        GetWindow<Yelby_Environment>("Yelby Environment");
    }

    private void OnGUI()
    {
        GUILayout.Label("Version [1.0]", EditorStyles.boldLabel);

        /*Select Avatar*/
        SDK = EditorGUILayout.ObjectField("Avatar", SDK, typeof(VRC.SDK3.Avatars.Components.VRCAvatarDescriptor), true) as VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
        if(SDK != null)
            avatar = SDK.gameObject;

        /*Check for humanoid*/
        if (avatar.GetComponent<Animator>().isHuman)
            GUILayout.Label("Avatar Type: Humanoid");
        else
            GUILayout.Label("Avatar Type: ERROR");

        if (!SDK.customizeAnimationLayers)
        {
            if (GUILayout.Button("Enable Custom Layers"))
                SDK.customizeAnimationLayers = true;
        }
        else
        {
            /*Base*/
            bBase = EditorGUILayout.Foldout(bBase, "Base");
            if (bBase)
            {
                int layer = 0;
                cBase = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                if (cBase == null)
                {
                    if (GUILayout.Button("Generate Default Controller"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar.name, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        cBase = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        //FillController(cBase);
                    }
                }
                else if (cBase != null)
                {
                    SDK.baseAnimationLayers[layer].isDefault = false;
                    SDK.baseAnimationLayers[layer].animatorController = cBase;
                    if (GUILayout.Button("Reset"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }

                    if (GUILayout.Button("Delete Controller"))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cBase));
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }
                }
            }

            /*Additive*/
            bAdditive = EditorGUILayout.Foldout(bAdditive, "Additive");
            if (bAdditive)
            {
                int layer = 1;
                cAdditive = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                if (cAdditive == null)
                {
                    if (GUILayout.Button("Generate Default Controller"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar.name, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        cAdditive = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        //FillController(cAdditive);
                    }
                }
                else if (cAdditive != null)
                {
                    SDK.baseAnimationLayers[layer].isDefault = false;
                    SDK.baseAnimationLayers[layer].animatorController = cAdditive;
                    if (GUILayout.Button("Reset"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }

                    if (GUILayout.Button("Delete Controller"))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cAdditive));
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }
                }
            }

            /*Gesture*/
            bGesture = EditorGUILayout.Foldout(bGesture, "Gesture");
            if (bGesture)
            {
                int layer = 2;
                cGesture = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                if (cGesture == null)
                {
                    if (GUILayout.Button("Generate Default Controller"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar.name, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        cGesture = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        //FillController(cGesture);
                    }
                }
                else if (cGesture != null)
                {
                    SDK.baseAnimationLayers[layer].isDefault = false;
                    SDK.baseAnimationLayers[layer].animatorController = cGesture;
                    if (GUILayout.Button("Reset"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }

                    if (GUILayout.Button("Delete Controller"))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cGesture));
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }
                }
            }

            /*Action*/
            bAction = EditorGUILayout.Foldout(bAction, "Action");
            if (bAction)
            {
                int layer = 3;
                cAction = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                if (cAction == null)
                {
                    if (GUILayout.Button("Generate Default Controller"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar.name, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        cAction = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        //FillController(cAction);
                    }
                }
                else if (cAction != null)
                {
                    SDK.baseAnimationLayers[layer].isDefault = false;
                    SDK.baseAnimationLayers[layer].animatorController = cAction;
                    if (GUILayout.Button("Reset"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }

                    if (GUILayout.Button("Delete Controller"))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cAction));
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }
                }
            }

            /*FX*/
            bFX = EditorGUILayout.Foldout(bFX, "FX");
            if (bFX)
            {
                int layer = 4;
                cFX = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                if (cFX == null)
                {
                    if (GUILayout.Button("Generate Default Controller"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar.name, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        cFX = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        FillController(cFX, "fx", avatar);
                    }
                }
                else if (cFX != null)
                {
                    SDK.baseAnimationLayers[layer].isDefault = false;
                    SDK.baseAnimationLayers[layer].animatorController = cFX;
                    
                    List<AnimationClip> clips = new List<AnimationClip>();
                    List<Motion> states = new List<Motion>();
                    int current = 0;

                    scrollLocation = GUILayout.BeginScrollView(scrollLocation);
                    //Travel layers for gather
                    for (int i = 0; i < cFX.layers.Length; i++)
                    {
                        if(cFX.layers[i].name != "Reset")
                        {
                            GUILayout.Label(cFX.layers[i].name);

                            //Travel states in a layer
                            for(int j = 0; j < cFX.layers[i].stateMachine.states.Length; j++)
                            {
                                GUILayout.BeginVertical();
                                //Motion m = cFX.layers[i].stateMachine.states[j].state.motion;
                                states.Add(cFX.layers[i].stateMachine.states[j].state.motion);
                                states[current] = EditorGUILayout.ObjectField(cFX.layers[i].stateMachine.states[j].state.name, (Motion)cFX.layers[i].stateMachine.states[j].state.motion, typeof(Motion), true) as Motion;
                                GUILayout.EndVertical();
                                current++;
                            }
                        }
                    }

                    //Travel layers for replace
                    current = 0;
                    for (int i = 0; i < cFX.layers.Length; i++)
                    {
                        if (cFX.layers[i].name != "Reset")
                        {
                            //Travel states in a layer
                            for (int j = 0; j < cFX.layers[i].stateMachine.states.Length; j++)
                            {
                                GUILayout.BeginVertical();
                                //Motion m = cFX.layers[i].stateMachine.states[j].state.motion;
                                cFX.layers[i].stateMachine.states[j].state.motion = states[current];
                                GUILayout.EndVertical();
                                current++;
                            }
                        }
                    }

                    GUILayout.EndScrollView();

                    if (GUILayout.Button("Reset"))
                    {
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }

                    if (GUILayout.Button("Delete Controller"))
                    {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(cFX));
                        SDK.baseAnimationLayers[layer].animatorController = null;
                        SDK.baseAnimationLayers[layer].isDefault = true;
                    }
                }
            }
        }
    }

    /*~~~~~Methods~~~~~*/
    private AnimatorController CreateController(string avatarName, string type)
    {
        string path = "Assets/Yelby/Programs/Yelby Environment";
        CreateFolder(path, avatarName);

        switch (type)
        {
            case "base":
                return GenerateController(avatarName, type, path);
            case "additive":
                return GenerateController(avatarName, type, path);
            case "gesture":
                return GenerateController(avatarName, type, path);
            case "action":
                return GenerateController(avatarName, type, path);
            case "fx":
                return GenerateController(avatarName, type, path);
        }
        return null;
    }

    private void FillController(AnimatorController controller, string type, GameObject avatar)
    {
        AnimatorControllerLayer[] layers = controller.layers;
        switch (type)
        {
            case "fx":
                controller.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
                controller.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Int);
                controller.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
                controller.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Int);

                if (layers.Length == 0)
                    controller.AddLayer("Reset");
                AssetDatabase.Refresh();

                int index = LayerIndex(layers, "Reset");
                layers = controller.layers;
                AnimatorStateMachine states = layers[index].stateMachine;
                Vector3 location = new Vector3(0, 0);
                states.anyStatePosition = location;
                location[1] += 50;
                states.entryPosition = location;
                location[1] += 50;
                states.exitPosition = location;

                /*Motion*/
                Motion animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/Reset.anim", typeof(Motion)) as Motion;
                if (animation == null)
                {
                    CreateAnimationReset("Assets/Yelby/Programs/Yelby Environment", avatar);
                    animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/Reset.anim", typeof(Motion)) as Motion;
                }

                /*State*/
                AnimatorState reset = createState(animation, states, new Vector3(200, 50));

                /*Left Hand Layer*/
                controller.AddLayer("Left Hand");
                layers = controller.layers;
                index = LayerIndex(layers, "Left Hand");
                var selectedLayer = layers[index];
                states = layers[index].stateMachine;
                location = new Vector3(0, 0);
                states.entryPosition = location;
                location[1] += 50;
                states.anyStatePosition = location;
                location[1] += 50;
                states.exitPosition = location;
                selectedLayer.defaultWeight = 1;
                controller.layers = layers;

                animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/BROKEN.anim", typeof(Motion)) as Motion;
                if (animation == null)
                {
                    CreateAnimationBroken("Assets/Yelby/Programs/Yelby Environment", avatar);
                    animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/BROKEN.anim", typeof(Motion)) as Motion;
                }

                location = new Vector3(200, 0);
                List<AnimatorState> statesList = new List<AnimatorState>();
                    statesList.Add(createState(animation, states, location));

                string[] poses = {"First","Open","Point","Peace","Rock n Roll", "Gun", "Thumbs up"};

                location[1] += 50;
                for (int i = 0; i < poses.Length; i++)
                {
                    statesList.Add(createState(poses[i], states, location));
                    location[1] += 50;
                }

                for(int i = 0; i < statesList.Count; i++)
                {
                    createTransition(states, statesList[i], false, 0, AnimatorConditionMode.Equals, i, "GestureLeft");
                }

                /*Right Hand Layer*/
                controller.AddLayer("Right Hand");
                layers = controller.layers;
                index = LayerIndex(layers, "Right Hand");
                selectedLayer = layers[index];
                states = layers[index].stateMachine;
                location = new Vector3(0, 0);
                states.entryPosition = location;
                location[1] += 50;
                states.anyStatePosition = location;
                location[1] += 50;
                states.exitPosition = location;
                selectedLayer.defaultWeight = 1;
                controller.layers = layers;

                animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/BROKEN.anim", typeof(Motion)) as Motion;
                if (animation == null)
                {
                    CreateAnimationBroken("Assets/Yelby/Programs/Yelby Environment", avatar);
                    animation = AssetDatabase.LoadAssetAtPath("Assets/Yelby/Programs/Yelby Environment/" + avatar.name + "/BROKEN.anim", typeof(Motion)) as Motion;
                }

                location = new Vector3(200, 0);
                statesList = new List<AnimatorState>();
                statesList.Add(createState(animation, states, location));

                location[1] += 50;
                for (int i = 0; i < poses.Length; i++)
                {
                    statesList.Add(createState(poses[i], states, location));
                    location[1] += 50;
                }

                for (int i = 0; i < statesList.Count; i++)
                {
                    createTransition(states, statesList[i], false, 0, AnimatorConditionMode.Equals, i, "GestureLeft");
                }
                break;
        }
    }

    /*~~~~~Helper Methods~~~~~*/
    private void CreateFolder(string path, string newFolder)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            if(!AssetDatabase.IsValidFolder(path + "/" + newFolder))
                AssetDatabase.CreateFolder(path, newFolder);
                AssetDatabase.Refresh();
        }
        else
            Debug.Log(path + " does not exist");
    }

    private AnimatorController GenerateController(string avatarName, string type, string path)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath(path + "/" + avatarName + "/" + avatarName + "_" + type + ".controller", typeof(AnimatorController)) as AnimatorController;
        if (controller != null)
            return controller;

        controller = new AnimatorController();
        AssetDatabase.CreateAsset(controller, path + "/" + avatarName + "/" + avatarName + "_" + type + ".controller");

        AssetDatabase.Refresh();

        return controller;
    }

    private bool LayerContains(AnimatorControllerLayer[] layers, string tName)
    {
        if (layers.Length == 1)
            return true;

        for (int i = 0; i < layers.Length; i++)
            if (layers[i].name == tName)
                return true;
        return false;
    }

    private int LayerIndex(AnimatorControllerLayer[] layers, string tName)
    {
        if (layers.Length == 1 && layers[0].name == "Reset")
            return 0;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == tName)
            {
                return i;
            }
        }
        return 0;
    }

    /*~~~~~Animation Methods~~~~~*/
    private void CreateAnimationReset(string filepath, GameObject avatar)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false;

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        GameObject bodyMesh = null;
        for (int i = 0; i < avatar.transform.childCount; i++)
        {
            if (avatar.transform.GetChild(i).gameObject.name == "Body")
            {
                bodyMesh = avatar.transform.GetChild(i).gameObject;
                break;
            }
        }

        var blendShapes = bodyMesh.GetComponent<SkinnedMeshRenderer>().sharedMesh;
        Keyframe[] keys = new Keyframe[2];
        AnimationCurve curve;
        for (int i = 0; i < blendShapes.blendShapeCount; i++)
        {
            keys[0] = new Keyframe(0.0f, 0.0f);
            keys[1] = new Keyframe(0.01f, 0.0f);
            curve = new AnimationCurve(keys);

            string name = blendShapes.GetBlendShapeName(i);
            if (name.Contains("vrc."))
                continue;
            else if (name.Contains("~~"))
                continue;

            clip.SetCurve(bodyMesh.name, typeof(SkinnedMeshRenderer), "blendShape." + name , curve);
        }

        AssetDatabase.CreateAsset(clip, filepath + "/" + avatar.name + "/" + "Reset.anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateAnimationBroken(string filepath, GameObject avatar)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false;

        Keyframe[] keys = new Keyframe[1];
        AnimationCurve curve;
        GameObject bodyMesh = null;
        for (int i = 0; i < avatar.transform.childCount; i++)
        {
            if (avatar.transform.GetChild(i).gameObject.name == "Body")
            {
                bodyMesh = avatar.transform.GetChild(i).gameObject;
                break;
            }
        }

        keys[0] = new Keyframe(0, 0);
        curve = new AnimationCurve(keys);
        clip.SetCurve(bodyMesh.name, typeof(SkinnedMeshRenderer), "blendShape." + "I_AM_BROKEN_NO_TOUCH", curve);
        AssetDatabase.CreateAsset(clip, filepath + "/" + avatar.name + "/" + "BROKEN.anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //Single Object
    private void createAnimation(string filePath, GameObject obj, bool toggle)
    {
        Keyframe[] keys = new Keyframe[1];
        AnimationCurve curve;
        string objectPath = obj.transform.GetHierarchyPath(null);
        objectPath = objectPath.Substring(avatar.name.Length + 1, objectPath.Length - avatar.name.Length - 1);

        AnimationClip clip = new AnimationClip();
        clip.legacy = false;

        keys[0] = new Keyframe(0.0f, (toggle ? 1.0f : 0.0f));
        curve = new AnimationCurve(keys);
        clip.SetCurve(objectPath, typeof(GameObject), "m_IsActive", curve);
        AssetDatabase.CreateAsset(clip, filePath + "/" + obj.name + (toggle ? "_ON" : "_OFF") + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //Based on bool list
    private void createAnimation(string filePath, List<GameObject> tObjs, List<bool> listOther, string outfitName)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false;
        Keyframe[] keys = new Keyframe[1];
        AnimationCurve curve;

        for (int i = 0; i < tObjs.Count; i++)
        {
            string objectPath = tObjs[i].transform.GetHierarchyPath(null);
            keys[0] = new Keyframe(0.0f, (listOther[i] ? 1.0f : 0.0f));
            curve = new AnimationCurve(keys);
            objectPath = objectPath.Substring(avatar.name.Length + 1, objectPath.Length - avatar.name.Length - 1);
            clip.SetCurve(objectPath, typeof(GameObject), "m_IsActive", curve);
        }

        AssetDatabase.CreateAsset(clip, filePath + "/" + outfitName + "_Default" + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //All items off then bool decides
    private void createAnimation(string filePath, GameObject tObj, List<GameObject> tObjs, bool toggle)
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = false;
        Keyframe[] keys = new Keyframe[1];
        AnimationCurve curve;

        for (int i = 0; i < tObjs.Count; i++)
        {
            string objectPath = tObjs[i].transform.GetHierarchyPath(null);
            if (tObjs[i].name == tObj.name)
                keys[0] = new Keyframe(0.0f, (toggle ? 1.0f : 0.0f));
            else
                keys[0] = new Keyframe(0.0f, 0.0f);
            curve = new AnimationCurve(keys);
            objectPath = objectPath.Substring(avatar.name.Length + 1, objectPath.Length - avatar.name.Length - 1);
            clip.SetCurve(objectPath, typeof(GameObject), "m_IsActive", curve);
        }

        AssetDatabase.CreateAsset(clip, filePath + "/" + tObj.name + (toggle ? "_ON" : "_OFF") + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private AnimatorState createState(Motion motion, AnimatorStateMachine stateMachine, Vector3 location)
    {
        stateMachine.AddState(motion.name, location);
        int i = 0;
        for (i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.states[i].state.name == motion.name)
            {
                stateMachine.states[i].state.motion = motion;
                stateMachine.states[i].state.writeDefaultValues = false;
                break;
            }
        }
        return stateMachine.states[i].state;
    }

    private AnimatorState createState(string title, AnimatorStateMachine stateMachine, Vector3 location)
    {
        stateMachine.AddState(title, location);
        int i = 0;
        for (i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.states[i].state.name == title)
            {
                stateMachine.states[i].state.motion = null;
                stateMachine.states[i].state.writeDefaultValues = false;
                break;
            }
        }
        return stateMachine.states[i].state;
    }

    //Transition from one state to another
    private void createTransition(AnimatorState start, AnimatorState end, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, GameObject obj)
    {
        AnimatorStateTransition transition = start.AddTransition(end);
        transition.hasExitTime = exitTime;
        transition.duration = duration;
        transition.AddCondition(mode, threshold, obj.name);
    }

    //Transition from state to exit
    private void createTransition(AnimatorState stateToExit, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, GameObject obj)
    {
        AnimatorStateTransition tranExit = stateToExit.AddExitTransition();
        tranExit.hasExitTime = exitTime;
        tranExit.duration = duration;
        tranExit.AddCondition(mode, threshold, obj.name);
    }

    //Transition from ANY to state
    private void createTransition(AnimatorStateMachine anyToState, AnimatorState location, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, string outfitName)
    {
        var anyState = anyToState.AddAnyStateTransition(location);
        anyState.hasExitTime = exitTime;
        anyState.duration = duration;
        anyState.AddCondition(mode, threshold, outfitName);
    }
}
#endif