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
    Vector2 sBase;
    Vector2 sAdditive;
    Vector2 sGesture;
    Vector2 sAction;
    Vector2 sFX;

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
            /*Base*/
            bBase = EditorGUILayout.Foldout(bBase, "Base");
            if (bBase)
            {
                GetAnimationController(0, cBase, ref SDK, ref sBase);
            }

            /*Additive*/
            bAdditive = EditorGUILayout.Foldout(bAdditive, "Additive");
            if (bAdditive)
            {
                GetAnimationController(1, cAdditive, ref SDK, ref sAdditive);
            }

            /*Gesture*/
            bGesture = EditorGUILayout.Foldout(bGesture, "Gesture");
            if (bGesture)
            {
                GetAnimationController(2, cGesture, ref SDK, ref sGesture);
            }

            /*Action*/
            bAction = EditorGUILayout.Foldout(bAction, "Action");
            if (bAction)
            {
                GetAnimationController(3, cAction, ref SDK, ref sAction);
            }

            /*FX*/
            bFX = EditorGUILayout.Foldout(bFX, "FX");
            if (bFX)
            {
                GetAnimationController(4, cFX, ref SDK, ref sFX);
            }
        }
    }

    /*~~~~~Methods~~~~~*/
    private void GetAnimationController(int layerIndex, AnimatorController controller, ref VRC.SDK3.Avatars.Components.VRCAvatarDescriptor SDK, ref Vector2 scroll)
    {
        int layer = layerIndex;
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
                if (controller.layers[i].name != "Reset")
                {
                    GUILayout.Label(controller.layers[i].name);

                    //Travel states in a layer
                    for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
                    {
                        GUILayout.BeginVertical();
                        /*for(int a = 0; a < controller.layers[i].stateMachine.states[j].state.behaviours.Length; a++)
                            Debug.Log(controller.layers[i].stateMachine.states[j].state.behaviours[a]);*/
                        states.Add(controller.layers[i].stateMachine.states[j].state.motion);
                        states[current] = EditorGUILayout.ObjectField(controller.layers[i].stateMachine.states[j].state.name, (Motion)controller.layers[i].stateMachine.states[j].state.motion, typeof(Motion), true) as Motion;
                        GUILayout.EndVertical();
                        current++;
                    }
                }
            }

            //Travel layers for replace
            current = 0;
            for (int i = 0; i < controller.layers.Length; i++)
            {
                if (controller.layers[i].name != "Reset")
                {
                    //Travel states in a layer
                    for (int j = 0; j < controller.layers[i].stateMachine.states.Length; j++)
                    {
                        GUILayout.BeginVertical();
                        controller.layers[i].stateMachine.states[j].state.motion = states[current];
                        GUILayout.EndVertical();
                        current++;
                    }
                }
            }

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
                        DoTransition(animatorStateList[i], animatorStateList, type);
                    }

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
                    break;
                }
        }
    }

    private AnimatorStateMachine FillController(AnimatorStateMachine stateMachine, string type, List<AnimatorState> list)
    {
        switch(type)
        {
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
                        list.Add(createState(animation, stateMachine, location));
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
                        list.Add(createState(animation, stateMachine, location));
                        location[1] += 50;
                    }

                    break;
                }
        }
        return stateMachine;
    }

    private void DoTransition(AnimatorState state, List<AnimatorState> list, string type)
    {
        switch(type)
        {
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
                                string[] proxyAnimations = { "proxy_stand_wave",
                                                             "proxy_stand_clap",
                                                             "proxy_stand_point",
                                                             "proxy_stand_cheer",
                                                             "proxy_dance",
                                                             "proxy_backflip",
                                                             "proxy_stand_sadkick",
                                                             "proxy_die"};

                                int blendOut = StateIndex(list, "BlendOut Stand");
                                for (int i = 0; i < list.Count; i++)
                                {
                                    for(int j = 0; j < proxyAnimations.Length; j++)
                                    {
                                        if (list[i].name == proxyAnimations[j])
                                        {
                                            createTransition(state, list[i], false, 0.25f, AnimatorConditionMode.Equals, j+1, "VRCEmote");
                                            createTransition(list[i], list[blendOut], false, 0.25f, AnimatorConditionMode.NotEqual, j + 1, "VRCEmote");
                                            break;
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
                                string[] proxyAnimations = { "proxy_seated_raise_hand",
                                                             "proxy_seated_clap",
                                                             "proxy_seated_point",
                                                             "proxy_seated_laugh",
                                                             "proxy_seated_drum",
                                                             "proxy_seated_shake_fist",
                                                             "proxy_seated_disapprove",
                                                             "proxy_seated_disbelief"};

                                int blendOut = StateIndex(list, "BlendOut Sit");
                                for (int i = 0; i < list.Count; i++)
                                {
                                    for (int j = 0; j < proxyAnimations.Length; j++)
                                    {
                                        if (list[i].name == proxyAnimations[j])
                                        {
                                            createTransition(state, list[i], false, 0.25f, AnimatorConditionMode.Equals, j + 9, "VRCEmote");
                                            createTransition(list[i], list[blendOut], false, 0.25f, AnimatorConditionMode.NotEqual, j + 9, "VRCEmote");
                                            break;
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