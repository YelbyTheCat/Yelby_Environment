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

    bool bSitting;
    bool bTPose;
    bool bIKPose;

    /*Controllers*/
    AnimatorController cBase;
    AnimatorController cAdditive;
    AnimatorController cGesture;
    AnimatorController cAction;
    AnimatorController cFX;

    AnimatorController cSitting;
    AnimatorController cTPose;
    AnimatorController cIKPose;

    /*Scroll Views*/
    Vector2 sBase;
    Vector2 sAdditive;
    Vector2 sGesture;
    Vector2 sAction;
    Vector2 sFX;

    Vector2 sSitting;
    Vector2 sTPose;
    Vector2 sIKPose;

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
        if (SDK != null)
            avatar = SDK.gameObject;
        else
            return;

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
            string section = "base";
            /*Base*/
            bBase = EditorGUILayout.Foldout(bBase, "Base");
            if (bBase)
            {
                GetAnimationController(0, ref cBase, ref SDK, ref sBase, section);
            }

            /*Additive*/
            bAdditive = EditorGUILayout.Foldout(bAdditive, "Additive");
            if (bAdditive)
            {
                GetAnimationController(1, ref cAdditive, ref SDK, ref sAdditive, section);
            }

            /*Gesture*/
            bGesture = EditorGUILayout.Foldout(bGesture, "Gesture");
            if (bGesture)
            {
                GetAnimationController(2, ref cGesture, ref SDK, ref sGesture, section);
            }

            /*Action*/
            bAction = EditorGUILayout.Foldout(bAction, "Action");
            if (bAction)
            {
                GetAnimationController(3, ref cAction, ref SDK, ref sAction, section);
            }

            /*FX*/
            bFX = EditorGUILayout.Foldout(bFX, "FX");
            if (bFX)
            {
                GetAnimationController(4, ref cFX, ref SDK, ref sFX, section);
            }

            /*Sitting*/
            bSitting = EditorGUILayout.Foldout(bSitting, "Sitting");
            if(bSitting)
            {
                GetAnimationController(0, ref cSitting, ref SDK, ref sSitting, "special");
            }

            /*TPose*/
            bTPose = EditorGUILayout.Foldout(bTPose, "TPose");
            if (bTPose)
            {
                GetAnimationController(1, ref cTPose, ref SDK, ref sTPose, "special");
            }

            /*IKPose*/
            bIKPose = EditorGUILayout.Foldout(bIKPose, "IKPose");
            if (bIKPose)
            {
                GetAnimationController(2, ref cIKPose, ref SDK, ref sIKPose, "special");
            }
        }
    }

    /*~~~~~Methods~~~~~*/
    private void GetAnimationController(int layerIndex, ref AnimatorController controller, ref VRC.SDK3.Avatars.Components.VRCAvatarDescriptor SDK, ref Vector2 scroll, string animatorSection)
    {
        int layer = layerIndex;

        switch(animatorSection)
        {
            case "base":
                {
                    controller = EditorGUILayout.ObjectField(SDK.baseAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                    if (controller == null)
                    {
                        if (GUILayout.Button("Generate Default Controller"))
                        {
                            SDK.baseAnimationLayers[layer].animatorController = CreateController(avatar, SDK.baseAnimationLayers[layer].type.ToString().ToLower());
                            SDK.baseAnimationLayers[layer].isDefault = false;
                            controller = SDK.baseAnimationLayers[layer].animatorController as AnimatorController;
                        }
                    }
                    else if (controller != null)
                    {
                        SDK.baseAnimationLayers[layer].isDefault = false;
                        SDK.baseAnimationLayers[layer].animatorController = controller;

                        List<AnimationClip> clips = new List<AnimationClip>();
                        List<Motion> states = new List<Motion>();
                        int current = 0;

                        scroll = GUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(300));

                        //Travel layers for gather
                        for (int i = 0; i < controller.layers.Length; i++)
                        {
                            var currentLayer = controller.layers[i];
                            if (currentLayer.name == "Reset")
                            {
                                continue;
                            }

                            GUILayout.Label(controller.layers[i].name);

                            //Travel sub-states in layer
                            var currentSubStateMachine = currentLayer.stateMachine.stateMachines;
                            if (currentSubStateMachine.Length != 0)
                            {
                                for (int j = 0; j < currentSubStateMachine.Length; j++)
                                {
                                    GUILayout.Label(currentSubStateMachine[j].stateMachine.name);
                                    var currentSubState = currentSubStateMachine[j].stateMachine.states;

                                    //Sub state states
                                    for (int p = 0; p < currentSubState.Length; p++)
                                    {
                                        var currentStateInSubState = currentSubState[p].state;
                                        states.Add(currentStateInSubState.motion);
                                        states[current] = EditorGUILayout.ObjectField(currentStateInSubState.name, (Motion)currentStateInSubState.motion, typeof(Motion), true) as Motion;
                                        current++;
                                    }
                                    GUILayout.Label("");
                                }
                            }

                            //Travel states in layer
                            var currentState = controller.layers[i].stateMachine;
                            for (int j = 0; j < currentState.states.Length; j++)
                            {
                                var currState = currentState.states[j];
                                GUILayout.BeginVertical();
                                states.Add(currState.state.motion);
                                states[current] = EditorGUILayout.ObjectField(currState.state.name, (Motion)currState.state.motion, typeof(Motion), true) as Motion;
                                GUILayout.EndVertical();
                                current++;
                            }
                            GUILayout.Label("");
                        }

                        //Travel layers for replace
                        current = 0;
                        for (int i = 0; i < controller.layers.Length; i++)
                        {
                            var currentLayer = controller.layers[i];
                            if (controller.layers[i].name == "Reset")
                            {
                                continue;
                            }

                            //Travel sub-states in layer
                            var currentSubStateMachine = currentLayer.stateMachine.stateMachines;
                            if (currentSubStateMachine.Length != 0)
                            {
                                for (int j = 0; j < currentSubStateMachine.Length; j++)
                                {
                                    var currentSubState = currentSubStateMachine[j].stateMachine.states;

                                    //Sub state states
                                    for (int p = 0; p < currentSubState.Length; p++)
                                    {
                                        var currentStateInSubState = currentSubState[p].state;
                                        currentStateInSubState.motion = states[current];
                                        current++;
                                    }
                                }
                            }

                            //Travel states in a layer
                            for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
                            {
                                GUILayout.BeginVertical();
                                controller.layers[i].stateMachine.states[j].state.motion = states[current];
                                GUILayout.EndVertical();
                                current++;
                            }
                        }

                        SDK.baseAnimationLayers[layer].animatorController = controller;

                        if (GUILayout.Button("Reset"))
                        {
                            SDK.baseAnimationLayers[layer].animatorController = null;
                            SDK.baseAnimationLayers[layer].isDefault = true;
                        }

                        if (GUILayout.Button("Delete Controller"))
                        {
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(controller));
                            SDK.baseAnimationLayers[layer].animatorController = null;
                            SDK.baseAnimationLayers[layer].isDefault = true;
                        }
                        GUILayout.EndScrollView();
                        //AssetDatabase.Refresh();
                    }
                    break;
                }
            case "special":
                {
                    controller = EditorGUILayout.ObjectField(SDK.specialAnimationLayers[layer].animatorController, typeof(AnimatorController), true) as AnimatorController;
                    if (controller == null)
                    {
                        if (GUILayout.Button("Generate Default Controller"))
                        {
                            SDK.specialAnimationLayers[layer].animatorController = CreateController(avatar, SDK.specialAnimationLayers[layer].type.ToString().ToLower());
                            SDK.specialAnimationLayers[layer].isDefault = false;
                            controller = SDK.specialAnimationLayers[layer].animatorController as AnimatorController;
                        }
                    }
                    else if (controller != null)
                    {
                        SDK.specialAnimationLayers[layer].isDefault = false;
                        SDK.specialAnimationLayers[layer].animatorController = controller;

                        List<AnimationClip> clips = new List<AnimationClip>();
                        List<Motion> states = new List<Motion>();
                        int current = 0;

                        scroll = GUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(300));

                        //Travel layers for gather
                        for (int i = 0; i < controller.layers.Length; i++)
                        {
                            var currentLayer = controller.layers[i];
                            if (currentLayer.name == "Reset")
                            {
                                continue;
                            }

                            GUILayout.Label(controller.layers[i].name);

                            //Travel sub-states in layer
                            var currentSubStateMachine = currentLayer.stateMachine.stateMachines;
                            if (currentSubStateMachine.Length != 0)
                            {
                                for (int j = 0; j < currentSubStateMachine.Length; j++)
                                {
                                    GUILayout.Label(currentSubStateMachine[j].stateMachine.name);
                                    var currentSubState = currentSubStateMachine[j].stateMachine.states;

                                    //Sub state states
                                    for (int p = 0; p < currentSubState.Length; p++)
                                    {
                                        var currentStateInSubState = currentSubState[p].state;
                                        states.Add(currentStateInSubState.motion);
                                        states[current] = EditorGUILayout.ObjectField(currentStateInSubState.name, (Motion)currentStateInSubState.motion, typeof(Motion), true) as Motion;
                                        current++;
                                    }
                                    GUILayout.Label("");
                                }
                            }

                            //Travel states in layer
                            var currentState = controller.layers[i].stateMachine;
                            for (int j = 0; j < currentState.states.Length; j++)
                            {
                                var currState = currentState.states[j];
                                GUILayout.BeginVertical();
                                states.Add(currState.state.motion);
                                states[current] = EditorGUILayout.ObjectField(currState.state.name, (Motion)currState.state.motion, typeof(Motion), true) as Motion;
                                GUILayout.EndVertical();
                                current++;
                            }
                            GUILayout.Label("");
                        }

                        //Travel layers for replace
                        current = 0;
                        for (int i = 0; i < controller.layers.Length; i++)
                        {
                            var currentLayer = controller.layers[i];
                            if (controller.layers[i].name == "Reset")
                            {
                                continue;
                            }

                            //Travel sub-states in layer
                            var currentSubStateMachine = currentLayer.stateMachine.stateMachines;
                            if (currentSubStateMachine.Length != 0)
                            {
                                for (int j = 0; j < currentSubStateMachine.Length; j++)
                                {
                                    var currentSubState = currentSubStateMachine[j].stateMachine.states;

                                    //Sub state states
                                    for (int p = 0; p < currentSubState.Length; p++)
                                    {
                                        var currentStateInSubState = currentSubState[p].state;
                                        currentStateInSubState.motion = states[current];
                                        current++;
                                    }
                                }
                            }

                            //Travel states in a layer
                            for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
                            {
                                GUILayout.BeginVertical();
                                controller.layers[i].stateMachine.states[j].state.motion = states[current];
                                GUILayout.EndVertical();
                                current++;
                            }
                        }

                        if (GUILayout.Button("Reset"))
                        {
                            SDK.specialAnimationLayers[layer].animatorController = null;
                            SDK.specialAnimationLayers[layer].isDefault = true;
                        }

                        if (GUILayout.Button("Delete Controller"))
                        {
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(controller));
                            SDK.specialAnimationLayers[layer].animatorController = null;
                            SDK.specialAnimationLayers[layer].isDefault = true;
                        }

                        //SDK.baseAnimationLayers[layer].animatorController = controller;

                        GUILayout.EndScrollView();
                        //AssetDatabase.Refresh();
                    }
                    break;
                }
        }
        
    }

    private AnimatorController CreateController(GameObject avatar, string type)
    {
        string path = "Assets/Yelby/Programs/Yelby Environment";
        CreateFolder(path, avatar.name);

        switch (type)
        {
            case "base":
                return GenerateController(avatar.name, type, path, avatar);
            case "additive":
                return GenerateController(avatar.name, type, path, avatar);
            case "gesture":
                return GenerateController(avatar.name, type, path, avatar);
            case "action":
                return GenerateController(avatar.name, type, path, avatar);
            case "fx":
                return GenerateController(avatar.name, type, path, avatar);
            case "sitting":
                return GenerateController(avatar.name, type, path, avatar);
            case "tpose":
                return GenerateController(avatar.name, type, path, avatar);
            case "ikpose":
                return GenerateController(avatar.name, type, path, avatar);
            default:
                Debug.Log("NO controller");
                break;
        }
        return null;
    }

    private void FillController(AnimatorController controller, string type, GameObject avatar)
    {
        AnimatorControllerLayer[] layers = controller.layers;
        switch (type)
        {
            case "base":
                {
                    controller.AddParameter("VelocityX", AnimatorControllerParameterType.Float);
                    controller.AddParameter("VelocityY", AnimatorControllerParameterType.Float);
                    controller.AddParameter("VelocityZ", AnimatorControllerParameterType.Float);
                    controller.AddParameter("AngularY", AnimatorControllerParameterType.Float);
                    controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
                    controller.AddParameter("Upright", AnimatorControllerParameterType.Float);
                    controller.AddParameter("Seated", AnimatorControllerParameterType.Bool);
                    controller.AddParameter("AFK", AnimatorControllerParameterType.Bool);
                    controller.AddParameter("TrackingType", AnimatorControllerParameterType.Int);

                    if (layers.Length == 0)
                        controller.AddLayer("Locomotion");
                    layers = controller.layers;
                    AssetDatabase.Refresh();

                    List<AnimatorState> statesList = new List<AnimatorState>();
                    layers = controller.layers;
                    var index = LayerIndex(layers, "Locomotion");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.entryPosition = location;
                    location[1] += 50;
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.exitPosition = location;
                    controller.layers = layers;

                    location[0] = 200;
                    location[1] = 0;
                    /*Standing*/
                    string path = "Assets/VRCSDK/Examples3/Animation/BlendTrees/";
                    BlendTree animation = AssetDatabase.LoadAssetAtPath(path + "vrc_StandingLocomotion" + ".asset", typeof(BlendTree)) as BlendTree;
                    AnimatorState state = createState("Standing", states, location);
                    //state.name = "Standing";
                    state.motion = animation;
                    statesList.Add(state);
                    //CreateBlendTree();

                    /*Crouching*/
                    location[1] += 50;
                    animation = AssetDatabase.LoadAssetAtPath(path + "vrc_CrouchingLocomotion" + ".asset", typeof(BlendTree)) as BlendTree;
                    state = createState("Crouching", states, location);
                    state.motion = animation;
                    statesList.Add(state);

                    /*Prone*/
                    location[1] += 50;
                    animation = AssetDatabase.LoadAssetAtPath(path + "vrc_ProneLocomotion" + ".asset", typeof(BlendTree)) as BlendTree;
                    state = createState("Prone", states, location);
                    state.motion = animation;
                    statesList.Add(state);

                    /*SubState*/
                    location[0] += 250;
                    location[1] = 0;
                    var jumpAndFall = states.AddStateMachine("JumpAndFall", location);
                    jumpAndFall = FillController(jumpAndFall, jumpAndFall.name, statesList);

                    for (int i = 0; i < statesList.Count; i++)
                    {
                        DoTransition(statesList[i], statesList, layers[0], type);
                    }

                    AssetDatabase.Refresh();

                    break;
                }
            case "additive":
                {
                    controller.AddParameter("Upright", AnimatorControllerParameterType.Int);
                    controller.AddParameter("TrackingType", AnimatorControllerParameterType.Int);

                    if (layers.Length == 0)
                        controller.AddLayer("Idle");

                    layers = controller.layers;
                    AvatarMask mask = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/Masks/vrc_MusclesOnly.mask", typeof(AvatarMask)) as AvatarMask;
                    layers[0].avatarMask = mask;
                    layers[0].blendingMode = AnimatorLayerBlendingMode.Additive;
                    controller.layers = layers;
                    AssetDatabase.Refresh();

                    List<AnimatorState> statesList = new List<AnimatorState>();
                    layers = controller.layers;
                    var index = LayerIndex(layers, "Left Hand");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.entryPosition = location;
                    location[1] += 50;
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.exitPosition = location;
                    controller.layers = layers;

                    /*Upright Idle*/
                    location[0] = 200;
                    location[1] = 0;
                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
                    AnimatorState state = createState(AssetDatabase.LoadAssetAtPath(path + "proxy_idle" + ".anim", typeof(Motion)) as Motion, states, location);
                    state.name = "Upright Idle";
                    statesList.Add(state);

                    location[1] += 50;
                    statesList.Add(createState("Empty", states, location));

                    createTransition(statesList[0], statesList[1], false, 0.0f, AnimatorConditionMode.Equals, 6, "TrackingType");
                    createTransition(statesList[1], statesList[0], false, 0.0f, AnimatorConditionMode.NotEqual, 6, "TrackingType");

                    AssetDatabase.Refresh();

                    break;
                }
            case "gesture":
                {
                    controller.AddParameter("GestureLeft", AnimatorControllerParameterType.Int);
                    controller.AddParameter("GestureLeftWeight", AnimatorControllerParameterType.Int);
                    controller.AddParameter("GestureRight", AnimatorControllerParameterType.Int);
                    controller.AddParameter("GestureRightWeight", AnimatorControllerParameterType.Int);

                    if (layers.Length == 0)
                        controller.AddLayer("AllParts");

                    AssetDatabase.Refresh();

                    layers = controller.layers;
                    AvatarMask mask = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/Masks/vrc_HandsOnly.mask", typeof(AvatarMask)) as AvatarMask;
                    layers[0].avatarMask = mask;
                    controller.layers = layers;

                    /*Left Hand*/
                    controller.AddLayer("Left Hand");
                    layers = controller.layers;
                    var index = LayerIndex(layers, "Left Hand");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.entryPosition = location;
                    location[1] += 50;
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.exitPosition = location;
                    selectedLayer.defaultWeight = 1;
                    controller.layers = layers;

                    mask = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/Masks/vrc_Hand Left.mask", typeof(AvatarMask)) as AvatarMask;
                    if (mask == null)
                    {
                        Debug.LogError("Mask doesn't exit at location");
                        return;
                    }
                    selectedLayer.avatarMask = mask;


                    location = new Vector3(200, 0);
                    List<AnimatorState> statesList = new List<AnimatorState>();

                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
                    string[] poses = {"Idle", "First", "Open", "Point", "Peace", "Rock n Roll", "Gun", "Thumbs up" };
                    string[] animations = { "proxy_hands_idle",
                                            "proxy_hands_fist",
                                            "proxy_hands_open",
                                            "proxy_hands_point",
                                            "proxy_hands_peace",
                                            "proxy_hands_rock",
                                            "proxy_hands_gun",
                                            "proxy_hands_thumbs_up" };

                    location[1] = 0;
                    for (int i = 0; i < poses.Length; i++)
                    {
                        AnimatorState state = createState(AssetDatabase.LoadAssetAtPath(path + animations[i] + ".anim", typeof(Motion)) as Motion, states, location);
                        state.name = poses[i];
                        statesList.Add(state);
                        location[1] += 50;
                    }

                    for (int i = 0; i < statesList.Count; i++)
                    {
                        createTransition(states, statesList[i], false, 0, AnimatorConditionMode.Equals, i, "GestureLeft");
                    }

                    controller.layers = layers;

                    /*Right Hand*/
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

                    mask = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/Masks/vrc_Hand Right.mask", typeof(AvatarMask)) as AvatarMask;
                    if (mask == null)
                        Debug.LogError("Mask doesn't exit at location");
                    selectedLayer.avatarMask = mask;

                    location = new Vector3(200, 0);

                    for (int i = 0; i < poses.Length; i++)
                    {
                        AnimatorState state = createState(AssetDatabase.LoadAssetAtPath(path + animations[i] + ".anim", typeof(Motion)) as Motion, states, location);
                        state.name = poses[i];
                        statesList.Add(state);
                        location[1] += 50;
                    }

                    for (int i = 0; i < statesList.Count; i++)
                    {
                        createTransition(states, statesList[i], false, 0, AnimatorConditionMode.Equals, i, "GestureRight");
                    }

                    controller.layers = layers;

                    AssetDatabase.Refresh();

                    break;
                }
            case "action":
                {
                    controller.AddParameter("VRCEmote", AnimatorControllerParameterType.Int);
                    controller.AddParameter("LocomotionMode", AnimatorControllerParameterType.Int);
                    controller.AddParameter("AFK", AnimatorControllerParameterType.Bool);
                    controller.AddParameter("Seated", AnimatorControllerParameterType.Bool);

                    if (layers.Length == 0)
                        controller.AddLayer("Action");
                    AssetDatabase.Refresh();

                    int index = LayerIndex(layers, "Action");
                    layers = controller.layers;
                    AnimatorStateMachine states = layers[index].stateMachine;
                    Vector3 location = new Vector3(0, 0);
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.entryPosition = location;
                    location[0] = 1700;
                    states.exitPosition = location;

                    /*Motion*/
                    Motion animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_stand_still.anim", typeof(Motion)) as Motion;
                    if(animation == null)
                    {
                        Debug.LogError("proxy_stand_still.anim is not found");
                        return;
                    }

                    /*States*/
                    List<AnimatorState> animatorStateList = new List<AnimatorState>();
                    location[0] = 200;
                    location[1] = 50;

                    /*WaitForActionOrAFK*/
                    AnimatorState animationState = createState(animation, states, location);
                    animationState.name = "WaitForActionOrAFK";
                    animatorStateList.Add(animationState);

                    /*Prepare Standing*/
                    location[1] = 0;
                    animationState = createState(animation, states, location);
                    animationState.name = "Prepare Standing";

                        /*Playable Layer*/
                        var playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 1.0f;
                        playableLayerControl.blendDuration = 0.5f;

                        /*Tracking Control*/
                        var animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        var tNoChange = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.NoChange;
                        var tTracking = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking;
                        var tAnimation = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tAnimation;
                        animatorTrackingControl.trackingRightHand = tAnimation;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tAnimation;
                        animatorTrackingControl.trackingRightFingers = tAnimation;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    animatorStateList.Add(animationState);

                    /*FullBodyAnimations*/
                    location[0] += 250;
                    var fullBodyAnimations = states.AddStateMachine("FullBodyAnimations", location);
                    fullBodyAnimations = FillController(fullBodyAnimations, fullBodyAnimations.name, animatorStateList);

                    /*BlendOut Stand*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "BlendOut Stand";

                        /*Playable Layer*/
                        playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 0.0f;
                        playableLayerControl.blendDuration = 0.25f;
                    animatorStateList.Add(animationState);

                    /*Restore Tracking (stand)*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "Restore Tracking (stand)";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tTracking;
                        animatorTrackingControl.trackingRightFingers = tTracking;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    animatorStateList.Add(animationState);

                    /*Sit*/
                    location[0] = 450;
                    location[1] = 50;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_sit.anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, states, location);
                    animationState.name = "Sit";
                    animatorStateList.Add(animationState);

                    /*Prepare Sitting*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "Prepare Sitting";

                        /*Playable Layer*/
                        playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 1.0f;
                        playableLayerControl.blendDuration = 0.25f;

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tAnimation;
                        animatorTrackingControl.trackingRightHand = tAnimation;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tAnimation;
                        animatorTrackingControl.trackingRightFingers = tAnimation;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    animatorStateList.Add(animationState);

                    /*SittingAnimations*/
                    location[0] += 250;
                    var sittingAnimations = states.AddStateMachine("SittingAnimations", location);
                    sittingAnimations = FillController(sittingAnimations, sittingAnimations.name, animatorStateList);

                    /*BlendOut Stand*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "BlendOut Sit";

                        /*Playable Layer*/
                        playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 0.0f;
                        playableLayerControl.blendDuration = 0.25f;
                    animatorStateList.Add(animationState);

                    /*Restore Tracking (stand)*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "Restore Tracking (sit)";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tTracking;
                        animatorTrackingControl.trackingRightFingers = tTracking;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    animatorStateList.Add(animationState);

                    /*Afk Init*/
                    location[0] = 200;
                    location[1] = 100;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_afk.anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, states, location);
                    animationState.name = "Afk Init";

                        /*Playable Layer*/
                        playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 1.0f;
                        playableLayerControl.blendDuration = 1.0f;

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tAnimation;
                        animatorTrackingControl.trackingRightHand = tAnimation;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tAnimation;
                        animatorTrackingControl.trackingRightFingers = tAnimation;
                        animatorTrackingControl.trackingEyes = tAnimation;
                        animatorTrackingControl.trackingMouth = tAnimation;
                    animatorStateList.Add(animationState);

                    /*AFK*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "AFK";
                    animatorStateList.Add(animationState);

                    /*Blendout*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "BlendOut";

                        /*Playable Layer*/
                        playableLayerControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCPlayableLayerControl>();
                        playableLayerControl.layer = VRC.SDKBase.VRC_PlayableLayerControl.BlendableLayer.Action;
                        playableLayerControl.goalWeight = 1.0f;
                        playableLayerControl.blendDuration = 1.0f;

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tTracking;
                        animatorTrackingControl.trackingRightFingers = tTracking;
                        animatorTrackingControl.trackingEyes = tTracking;
                        animatorTrackingControl.trackingMouth = tTracking;
                    animatorStateList.Add(animationState);

                    for(int i = 0; i < animatorStateList.Count; i++)
                    {
                        DoTransition(animatorStateList[i], animatorStateList, layers[0], type);
                    }

                    AssetDatabase.Refresh();

                    break;
                }
            case "fx":
                {
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
                        createTransition(states, statesList[i], false, 0, AnimatorConditionMode.Equals, i, "GestureRight");
                    }

                    AssetDatabase.Refresh();

                    break;
                }
            case "sitting":
                {
                    if (layers.Length == 0)
                        controller.AddLayer("Utility layer");
                    layers = controller.layers;
                    AssetDatabase.Refresh();

                    List<AnimatorState> statesList = new List<AnimatorState>();
                    layers = controller.layers;
                    var index = LayerIndex(layers, "Utility layer");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.entryPosition = location;
                    controller.layers = layers;

                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";

                    /*VersionSwitch*/
                    location = new Vector3(200, 50);
                    Motion animation = AssetDatabase.LoadAssetAtPath(path + "proxy_stand_still" + ".anim", typeof(Motion)) as Motion;
                    AnimatorState animationState = createState(animation, states, location);
                    animationState.name = "VersionSwitch";
                    statesList.Add(animationState);

                    /*V2Seated*/
                    location[0] += 250;
                    animation = AssetDatabase.LoadAssetAtPath(path + "proxy_sit" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, states, location);
                    animationState.name = "V2Seated";
                    statesList.Add(animationState);

                    /*DisableTracking*/
                    location[0] -= 250;
                    location[1] += 50;
                    animation = AssetDatabase.LoadAssetAtPath(path + "proxy_stand_still" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, states, location);
                    animationState.name = "DisableTracking";

                        /*Tracking Control*/
                        var animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        var tNoChange = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.NoChange;
                        var tTracking = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking;
                        var tAnimation = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tAnimation;
                        animatorTrackingControl.trackingRightHand = tAnimation;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tTracking;
                        animatorTrackingControl.trackingRightFingers = tTracking;
                        animatorTrackingControl.trackingEyes = tTracking;
                        animatorTrackingControl.trackingMouth = tTracking;
                    statesList.Add(animationState);

                    /*PoseSpace*/
                    animation = AssetDatabase.LoadAssetAtPath(path + "proxy_sit" + ".anim", typeof(Motion)) as Motion;
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "PoseSpace";

                        /*Tracking Control*/
                        var tempPoseSpace = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTemporaryPoseSpace>();
                        tempPoseSpace.enterPoseSpace = true;
                        tempPoseSpace.fixedDelay = true;
                        tempPoseSpace.delayTime = 0.51f;
                    statesList.Add(animationState);

                    /*UpperBodyTracked*/
                    location[0] += 250;
                    animationState = createState(animation, states, location);
                    animationState.name = "UpperBodyTracked";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tNoChange;
                        animatorTrackingControl.trackingLeftFoot = tNoChange;
                        animatorTrackingControl.trackingRightFoot = tNoChange;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    statesList.Add(animationState);

                    /*RestoreTracking*/
                    location[0] += 250;
                    animation = AssetDatabase.LoadAssetAtPath(path + "proxy_stand_still" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, states, location);
                    animationState.name = "RestoreTracking";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;

                        /*Tracking Control*/
                        tempPoseSpace = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTemporaryPoseSpace>();
                        tempPoseSpace.enterPoseSpace = false;
                        tempPoseSpace.fixedDelay = true;
                        tempPoseSpace.delayTime = 0.0f;
                    statesList.Add(animationState);

                    location[1] -= 50;
                    states.exitPosition = location;

                    for(int i = 0; i < statesList.Count; i++)
                    {
                        DoTransition(statesList[i], statesList, selectedLayer, type);
                    }

                    break;
                }
            case "tpose":
                {
                    if (layers.Length == 0)
                        controller.AddLayer("Utility layer");
                    layers = controller.layers;
                    AssetDatabase.Refresh();

                    layers = controller.layers;
                    var index = LayerIndex(layers, "Utility Layer");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.entryPosition = location;
                    location[1] += 50;
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.exitPosition = location;
                    controller.layers = layers;
                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";

                    /*TPose*/
                    location = new Vector3(200, 0);
                    Motion animation = AssetDatabase.LoadAssetAtPath(path + "proxy_tpose" + ".anim", typeof(Motion)) as Motion;
                    AnimatorState animationState = createState(animation, states, location);
                    animationState.name = "TPose";
                    break;
                }
            case "ikpose":
                {
                    if (layers.Length == 0)
                        controller.AddLayer("Utility layer");
                    layers = controller.layers;
                    AssetDatabase.Refresh();

                    layers = controller.layers;
                    var index = LayerIndex(layers, "Utility Layer");
                    var selectedLayer = layers[index];
                    var states = layers[index].stateMachine;
                    var location = new Vector3(0, 0);
                    states.entryPosition = location;
                    location[1] += 50;
                    states.anyStatePosition = location;
                    location[1] += 50;
                    states.exitPosition = location;
                    controller.layers = layers;
                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";

                    /*IK Pose*/
                    location = new Vector3(200, 0);
                    Motion animation = AssetDatabase.LoadAssetAtPath(path + "proxy_ikpose" + ".anim", typeof(Motion)) as Motion;
                    AnimatorState animationState = createState(animation, states, location);
                    animationState.name = "IK Pose";
                    break;
                }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private AnimatorStateMachine FillController(AnimatorStateMachine stateMachine, string type, List<AnimatorState> list)
    {
        switch(type)
        {
            case "JumpAndFall":
                {
                    Vector3 location = new Vector3(0, 0);
                    stateMachine.anyStatePosition = location;
                    location[1] += 50;
                    stateMachine.exitPosition = new Vector3(700, 50);
                    stateMachine.entryPosition = location;
                    location[0] -= 25;
                    location[1] += 100;
                    stateMachine.parentStateMachinePosition = location;

                    /*Small Hop*/
                    location = new Vector3(200, 50);
                    Motion animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_fall_short" + ".anim", typeof(Motion)) as Motion;
                    AnimatorState animationState = createState(animation, stateMachine, location);
                    animationState.name = "SmallHop";

                        /*Tracking Control*/
                        var animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        var tNoChange = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.NoChange;
                        var tTracking = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Tracking;
                        var tAnimation = VRC.SDKBase.VRC_AnimatorTrackingControl.TrackingType.Animation;
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*Short Fall*/
                    location[1] += 100;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "Short Fall";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*RestoreToHop*/
                    location[0] += 250;
                    location[1] -= 50;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "RestoreToHop";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*QuickLand*/
                    location[1] += 100;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_land_quick" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "QuickLand";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tNoChange;
                        animatorTrackingControl.trackingRightHand = tNoChange;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*LongFall*/
                    location[0] -= 250;
                    location[1] += 50;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_fall_long" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "LongFall";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tAnimation;
                        animatorTrackingControl.trackingRightHand = tAnimation;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*HardLand*/
                    location[0] += 250*2;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_landing" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "HardLand";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tAnimation;
                        animatorTrackingControl.trackingLeftHand = tNoChange;
                        animatorTrackingControl.trackingRightHand = tNoChange;
                        animatorTrackingControl.trackingHip = tAnimation;
                        animatorTrackingControl.trackingLeftFoot = tAnimation;
                        animatorTrackingControl.trackingRightFoot = tAnimation;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    /*RestoreTracking*/
                    location[1] -= 100;
                    animation = AssetDatabase.LoadAssetAtPath("Assets/VRCSDK/Examples3/Animation/ProxyAnim/" + "proxy_stand_still" + ".anim", typeof(Motion)) as Motion;
                    animationState = createState(animation, stateMachine, location);
                    animationState.name = "RestoreTracking";

                        /*Tracking Control*/
                        animatorTrackingControl = animationState.AddStateMachineBehaviour<VRC.SDK3.Avatars.Components.VRCAnimatorTrackingControl>();
                        animatorTrackingControl.trackingHead = tTracking;
                        animatorTrackingControl.trackingLeftHand = tTracking;
                        animatorTrackingControl.trackingRightHand = tTracking;
                        animatorTrackingControl.trackingHip = tTracking;
                        animatorTrackingControl.trackingLeftFoot = tTracking;
                        animatorTrackingControl.trackingRightFoot = tTracking;
                        animatorTrackingControl.trackingLeftFingers = tNoChange;
                        animatorTrackingControl.trackingRightFingers = tNoChange;
                        animatorTrackingControl.trackingEyes = tNoChange;
                        animatorTrackingControl.trackingMouth = tNoChange;
                    list.Add(animationState);

                    break;
                }
            case "FullBodyAnimations":
                {
                    Vector3 location = new Vector3(0, 0);
                    stateMachine.entryPosition = location;
                    location[1] += 50;
                    stateMachine.anyStatePosition = location;
                    location[1] += 50;
                    stateMachine.exitPosition = location;

                    List<Motion> animations = new List<Motion>();
                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
                    string[] proxyAnimations = { "proxy_stand_wave",
                                                 "proxy_stand_clap",
                                                 "proxy_stand_point",
                                                 "proxy_stand_cheer",
                                                 "proxy_dance",
                                                 "proxy_backflip",
                                                 "proxy_stand_sadkick",
                                                 "proxy_die"};

                    Motion animation;
                    location[0] = 200;
                    location[1] = 0;
                    for (int i = 0; i < proxyAnimations.Length; i++)
                    {
                        animation = AssetDatabase.LoadAssetAtPath(path + proxyAnimations[i] + ".anim", typeof(Motion)) as Motion;
                        AnimatorState subState = createState(animation, stateMachine, location);
                        subState.name = "VRCEmote " + (i + 1);
                        list.Add(subState);
                        location[1] += 50;

                    }
                    
                    break;
                }
            case "SittingAnimations":
                {
                    Vector3 location = new Vector3(0, 0);
                    stateMachine.entryPosition = location;
                    location[1] += 50;
                    stateMachine.anyStatePosition = location;
                    location[1] += 50;
                    stateMachine.exitPosition = location;

                    List<Motion> animations = new List<Motion>();
                    string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
                    string[] proxyAnimations = { "proxy_seated_raise_hand",
                                                 "proxy_seated_clap",
                                                 "proxy_seated_point",
                                                 "proxy_seated_laugh",
                                                 "proxy_seated_drum",
                                                 "proxy_seated_shake_fist",
                                                 "proxy_seated_disapprove",
                                                 "proxy_seated_disbelief"};

                    Motion animation;
                    location[0] = 200;
                    location[1] = 0;
                    for (int i = 0; i < proxyAnimations.Length; i++)
                    {
                        animation = AssetDatabase.LoadAssetAtPath(path + proxyAnimations[i] + ".anim", typeof(Motion)) as Motion;
                        AnimatorState subState = createState(animation, stateMachine, location);
                        subState.name = "VRCEmote " + (i + 9);
                        list.Add(subState);
                        location[1] += 50;
                    }

                    break;
                }
        }
        return stateMachine;
    }

    private void DoTransition(AnimatorState state, List<AnimatorState> list, AnimatorControllerLayer layer, string type)
    {
        switch(type)
        {
            case "base":
                {
                    switch (state.name)
                    {
                        case "Standing":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Crouching")
                                    {
                                        createTransition(state, list[i], false, 0.5f, AnimatorConditionMode.Less, 0.68f, "Upright");
                                    }
                                    else if (list[i].name == "SmallHop")
                                    {
                                        AnimatorStateTransition transition = state.AddTransition(list[i]);
                                        transition.hasExitTime = false;
                                        transition.duration = 0.1f;
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
                                        transition.AddCondition(AnimatorConditionMode.Greater, -2.001f, "VelocityY");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Seated");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "AFK");
                                    }
                                    else if (list[i].name == "Short Fall")
                                    {
                                        AnimatorStateTransition transition = state.AddTransition(list[i]);
                                        transition.hasExitTime = false;
                                        transition.duration = 0.1f;
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
                                        transition.AddCondition(AnimatorConditionMode.Greater, -4f, "VelocityY");
                                        transition.AddCondition(AnimatorConditionMode.Less, -2f, "VelocityY");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Seated");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "AFK");
                                    }
                                    else if (list[i].name == "LongFall")
                                    {
                                        AnimatorStateTransition transition = state.AddTransition(list[i]);
                                        transition.hasExitTime = false;
                                        transition.duration = 0.2f;
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
                                        transition.AddCondition(AnimatorConditionMode.Less, -20f, "VelocityY");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "Seated");
                                        transition.AddCondition(AnimatorConditionMode.IfNot, 0, "AFK");
                                    }
                                }
                                break;
                            }
                        case "Crouching":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Prone")
                                    {
                                        createTransition(state, list[i], false, 0.5f, AnimatorConditionMode.Less, 0.41f, "Upright");
                                    }
                                    else if (list[i].name == "Standing")
                                    {
                                        createTransition(state, list[i], false, 0.2f, AnimatorConditionMode.Greater, 0.7f, "Upright");
                                    }
                                }
                                break;
                            }
                        case "Prone":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Crouching")
                                    {
                                        createTransition(state, list[i], false, 0.5f, AnimatorConditionMode.Greater, 0.43f, "Upright");
                                    }
                                }
                                break;
                            }
                            /*SubStates*/
                        case "SmallHop":
                            {
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "Grounded");
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "Seated");
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "AFK");
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Short Fall")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Less, -2f, "VelocityY");
                                        break;
                                    }
                                }
                                break;
                            }
                        case "Short Fall":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "RestoreTracking")
                                    {
                                        createTransition(state, list[i], false, 0.1f, AnimatorConditionMode.If, 0, "Seated");
                                        createTransition(state, list[i], false, 0.1f, AnimatorConditionMode.If, 0, "AFK");
                                    }
                                    else if (list[i].name == "LongFall")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Less, -20f, "VelocityY");
                                    }
                                    else if (list[i].name == "RestoreToHop")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Greater, 0.01f, "VelocityY");
                                    }
                                    else if (list[i].name == "QuickLand")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.If, 0, "Grounded");
                                    }
                                }
                                break;
                            }
                        case "LongFall":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "HardLand")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.If, 0, "Grounded");
                                    }
                                    else if (list[i].name == "Short Fall")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Greater, -20f, "VelocityY");
                                    }
                                    else if (list[i].name == "RestoreTracking")
                                    {
                                        createTransition(state, list[i], false, 0.1f, AnimatorConditionMode.If, 0, "AFK");
                                        createTransition(state, list[i], false, 0.1f, AnimatorConditionMode.If, 0, "Seated");
                                    }
                                }
                                break;
                            }
                        case "RestoreToHop":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Short Fall")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Less, -2.0f, "VelocityY");
                                        break;
                                    }
                                }
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "AFK");
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "Seated");
                                createTransition(state, false, 0.25f, AnimatorConditionMode.If, 0, "Grounded");
                                break;
                            }
                        case "QuickLand":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "RestoreTracking")
                                    {
                                        createTransition(state, list[i], true, 1.0f, 0.1f);
                                        break;
                                    }
                                }
                                break;
                            }
                        case "RestoreTracking":
                            {
                                createTransition(state, true, 0.0f, 0.25f);
                                break;
                            }
                        case "HardLand":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "RestoreTracking")
                                    {
                                        createTransition(state, list[i], true, 0.6f, 0.2f);
                                        break;
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
            case "action":
                {
                    switch(state.name)
                    {
                        case "WaitForActionOrAFK":
                            {
                                for(int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Prepare Standing")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Greater, 0, "VRCEmote",
                                                                                      AnimatorConditionMode.Less, 9, "VRCEmote");
                                    }
                                    else if(list[i].name == "Sit")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.If, 1, "Seated");
                                    }
                                    else if (list[i].name == "Afk Init")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.If, 1, "AFK");
                                    }
                                }
                                break;
                            }
                        case "Prepare Standing":
                            {
                                int blendOut = StateIndex(list, "BlendOut Stand");
                                for (int i = 0; i < list.Count; i++)
                                {
                                    for (int j = 0; j < layer.stateMachine.stateMachines.Length; j++)
                                    {
                                        if (layer.stateMachine.stateMachines[j].stateMachine.name == "FullBodyAnimations")
                                        {
                                            for (int p = 0; p < layer.stateMachine.stateMachines[j].stateMachine.states.Length; p++)
                                            {
                                                if (list[i].name == "VRCEmote " + (p + 1))
                                                {
                                                    createTransition(state, list[i], false, 0.25f, AnimatorConditionMode.Equals, p + 1, "VRCEmote");
                                                    createTransition(list[i], list[blendOut], false, 0.25f, AnimatorConditionMode.NotEqual, p + 1, "VRCEmote");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                        case "BlendOut Stand":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Restore Tracking (stand)")
                                    {
                                        createTransition(state, list[i], true, 1.0f, 0.25f);
                                        break;
                                    }
                                }
                                break;
                            }

                        case "Restore Tracking (stand)":
                            {
                                createTransition(state, true, 1.0f, 0.0f);
                                break;
                            }

                        case "Sit":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Prepare Sitting")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.Greater, 8, "VRCEmote",
                                                                                      AnimatorConditionMode.Less, 17, "VRCEmote");
                                    }
                                    else if(list[i].name == "Afk Init")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.If, 1, "AFK");
                                    }
                                }
                                break;
                            }

                        case "Prepare Sitting":
                            {
                                int blendOut = StateIndex(list, "BlendOut Sit");
                                for (int i = 0; i < list.Count; i++)
                                {
                                    for (int j = 0; j < layer.stateMachine.stateMachines.Length; j++)
                                    {
                                        if(layer.stateMachine.stateMachines[j].stateMachine.name == "SittingAnimations")
                                        {
                                            for(int p = 0; p < layer.stateMachine.stateMachines[j].stateMachine.states.Length; p++)
                                            {
                                                if (list[i].name == "VRCEmote " + (p + 9))
                                                {
                                                    createTransition(state, list[i], false, 0.25f, AnimatorConditionMode.Equals, p + 9, "VRCEmote");
                                                    createTransition(list[i], list[blendOut], false, 0.25f, AnimatorConditionMode.NotEqual, p + 9, "VRCEmote");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            }

                        case "BlendOut Sit":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "Restore Tracking (sit)")
                                    {
                                        createTransition(state, list[i], true, 1.0f, 0.25f);
                                        break;
                                    }
                                }
                                break;
                            }

                        case "Restore Tracking (sit)":
                            {
                                createTransition(state, true, 1.0f, 0.0f);
                                break;
                            }

                        case "Afk Init":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "AFK")
                                    {
                                        createTransition(state, list[i], true, 0.01f, 1.0f, true);
                                        break;
                                    }
                                }
                                break;
                            }

                        case "AFK":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "BlendOut")
                                    {
                                        createTransition(state, list[i], false, 0.0f, AnimatorConditionMode.IfNot, 0, "AFK");
                                        break;
                                    }
                                }
                                break;
                            }

                        case "BlendOut":
                            {
                                createTransition(state, true, 0.2f, 0.0f);
                                break;
                            }
                    }
                    break;
                }
            case "sitting":
                {
                    switch(state.name)
                    {
                        case "VersionSwitch":
                            {
                                for(int i = 0; i < list.Count; i++)
                                {
                                    if(list[i].name == "V2Seated")
                                    {
                                        createTransition(state, list[i], false, 0.2f, AnimatorConditionMode.Less, 3, "AvatarVersion", AnimatorConditionMode.If, 0, "Seated");
                                    }
                                    else if (list[i].name == "DisableTracking")
                                    {
                                        createTransition(state, list[i], false, 0.2f, AnimatorConditionMode.Greater, 2, "AvatarVersion", AnimatorConditionMode.If, 0, "Seated");
                                    }
                                }
                                break;
                            }
                        case "V2Seated":
                            {
                                createTransition(state, false, 0.2f, AnimatorConditionMode.IfNot, 0, "Seated");
                                break;
                            }
                        case "DisableTracking":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "PoseSpace")
                                    {
                                        createTransition(state, list[i], true, 1, 0.5f);
                                    }
                                }
                                break;
                            }
                        case "PoseSpace":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "UpperBodyTracked")
                                    {
                                        createTransition(state, list[i], true, 1, 0.2f);
                                    }
                                }
                                break;
                            }
                        case "UpperBodyTracked":
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].name == "RestoreTracking")
                                    {
                                        createTransition(state, list[i], false, 0, AnimatorConditionMode.IfNot, 0, "Seated");
                                    }
                                }
                                break;
                            }
                        case "RestoreTracking":
                            {
                                createTransition(state, true, 1, 0.2f);
                                break;
                            }
                    }
                    break;
                }
        }
    }

    /*~~~~~Helper Methods~~~~~*/
    private void CreateBlendTree()
    {
        string filePath = "Assets/Yelby/Programs/Yelby Environment";
        BlendTree tree = new BlendTree();
        tree.blendParameter = "VelocityX";
        tree.blendParameterY = "VelocityZ";
        tree.blendType = BlendTreeType.FreeformDirectional2D;

        string path = "Assets/VRCSDK/Examples3/Animation/ProxyAnim/";
        Motion animation = AssetDatabase.LoadAssetAtPath(path + "proxy_sprint_forward" + ".anim", typeof(Motion)) as Motion;
        tree.AddChild(animation, new Vector2(0, 5.96f));

        animation = AssetDatabase.LoadAssetAtPath(path + "proxy_run_forward" + ".anim", typeof(Motion)) as Motion;
        tree.AddChild(animation, new Vector2(0, 3.4f));

        Motion[] animationList = { AssetDatabase.LoadAssetAtPath(path + "proxy_sprint_forward" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_forward" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_walk_forward" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_stand_still" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_walk_backward" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_backward" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right_135" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right_135" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right_45" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_strafe_right_45" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right_45" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right_45" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right_135" + ".anim", typeof(Motion)) as Motion,
                                   AssetDatabase.LoadAssetAtPath(path + "proxy_run_strafe_right_135" + ".anim", typeof(Motion)) as Motion};
        Vector2[] vectorList = {    new Vector2(0f, 5.96f),
                                    new Vector2(0f, 3.4f),
                                    new Vector2(0f, 1.56f),
                                    new Vector2(0f, 0f),
                                    new Vector2(0f, -1.56f),
                                    new Vector2(0f, -2.1f),
                                    new Vector2(-3f, 0f),
                                    new Vector2(-1.56f, 0f),
                                    new Vector2(1.56f, 0f),
                                    new Vector2(3f, 0f),
                                    new Vector2(-1.1f, -1.1f),
                                    new Vector2(1.1f, -1.1f),
                                    new Vector2(-1.1f, 1.1f),
                                    new Vector2(1.1f, 1.1f),
                                    new Vector2(-2.44f, 2.44f),
                                    new Vector2(2.4f, 2.44f),
                                    new Vector2(-1.5f, -1.5f),
                                    new Vector2(1.5f, -1.5f)};

        for(int i = 0; i < vectorList.Length; i++)
        {
            tree.AddChild(animationList[i], vectorList[i]);
            if(i == 3)
            {
                tree.children[i].timeScale = 0.166f;
            }
        }

        AssetDatabase.CreateAsset(tree, filePath + "/" + "standing" + ".controller");
    }

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

    private AnimatorController GenerateController(string avatarName, string type, string path, GameObject avatar)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath(path + "/" + avatarName + "/" + avatarName + "_" + type + ".controller", typeof(AnimatorController)) as AnimatorController;
        if (controller != null)
        {
            return controller;
        }

        controller = new AnimatorController();
        AssetDatabase.CreateAsset(controller, path + "/" + avatarName + "/" + avatarName + "_" + type + ".controller");

        FillController(controller, type, avatar);

        AssetDatabase.SaveAssets();
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

    private int StateIndex(List<AnimatorState> state, string tName)
    {
        for (int i = 0; i < state.Count; i++)
        {
            if (state[i].name == tName)
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

    private AnimatorState createState(BlendTree motion, AnimatorStateMachine stateMachine, Vector3 location)
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

    private void createTransition(AnimatorState start, AnimatorState end, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, string parameter, AnimatorConditionMode mode2, float threshold2, string parameter2)
    {
        AnimatorStateTransition transition = start.AddTransition(end);
        transition.hasExitTime = exitTime;
        transition.duration = duration;
        transition.AddCondition(mode, threshold, parameter);
        transition.AddCondition(mode2, threshold2, parameter2);
    }

    private void createTransition(AnimatorState start, AnimatorState end, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, string parameter)
    {
        AnimatorStateTransition transition = start.AddTransition(end);
        transition.hasExitTime = exitTime;
        transition.duration = duration;
        transition.AddCondition(mode, threshold, parameter);
    }

    private void createTransition(AnimatorState start, AnimatorState end, bool exitTime, float timeToExit, float duration)
    {
        AnimatorStateTransition transition = start.AddTransition(end);
        transition.hasExitTime = exitTime;
        transition.exitTime = timeToExit;
        transition.duration = duration;
    }

    private void createTransition(AnimatorState start, AnimatorState end, bool exitTime, float timeToExit, float duration, bool interruption)
    {
        AnimatorStateTransition transition = start.AddTransition(end);
        transition.hasExitTime = exitTime;
        transition.exitTime = timeToExit;
        transition.duration = duration;
        transition.interruptionSource = TransitionInterruptionSource.DestinationThenSource;
        transition.orderedInterruption = interruption;
    }

    //Transition from state to exit
    private void createTransition(AnimatorState stateToExit, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, GameObject obj)
    {
        AnimatorStateTransition tranExit = stateToExit.AddExitTransition();
        tranExit.hasExitTime = exitTime;
        tranExit.duration = duration;
        tranExit.AddCondition(mode, threshold, obj.name);
    }

    private void createTransition(AnimatorState stateToExit, bool exitTime, float duration, AnimatorConditionMode mode, float threshold, string parameter)
    {
        AnimatorStateTransition tranExit = stateToExit.AddExitTransition();
        tranExit.hasExitTime = exitTime;
        tranExit.duration = duration;
        tranExit.AddCondition(mode, threshold, parameter);
    }

    private void createTransition(AnimatorState stateToExit, bool exitTime, float timeToExit, float duration)
    {
        AnimatorStateTransition tranExit = stateToExit.AddExitTransition();
        tranExit.hasExitTime = exitTime;
        tranExit.exitTime = timeToExit;
        tranExit.duration = duration;
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