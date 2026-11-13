using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using Il2CppReloaded.TreeStateActivities;
using Il2CppSource.TreeStateActivities;
using Il2CppTekly.TreeState;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using States = Il2CppSystem.Collections.Generic.List<Il2CppTekly.TreeState.TreeState>;

namespace FusionMod;

// The error view belongs to the mod, independently of the refused or unloaded
// gameplay activity. It owns no camera, RenderTexture or native model binder.
internal static class FusionSaveRecoveryUi
{
    private static FusionSaveCoordinator.OwnerState? owner;
    private static GameObject? root, previousSelection;
    private static Button? retry;
    private static bool requested;
    private static readonly List<UnityEngine.Events.UnityAction> listeners = new();
    private static readonly List<UnityEngine.Events.UnityAction<BaseEventData>> cancelListeners = new();
    internal static void Show(FusionSaveCoordinator.OwnerState value) { owner = value; requested = true; }
    internal static void Hide()
    {
        if (root != null) Object.Destroy(root); root = null; retry = null; requested = false; listeners.Clear(); cancelListeners.Clear();
        if (previousSelection != null && previousSelection.activeInHierarchy && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(previousSelection);
        previousSelection = null;
    }
    internal static void Update()
    {
        FusionSaveLifecycle.Update();
        if (!requested || owner == null) return;
        if (!FusionSaveCoordinator.IsCurrent(owner)) { Hide(); owner = null; return; }
        if (root != null)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null && retry != null)
                EventSystem.current.SetSelectedGameObject(retry.gameObject);
            return;
        }
        root = new GameObject("Fusion save recovery"); Object.DontDestroyOnLoad(root);
        var canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32760;
        root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        root.AddComponent<GraphicRaycaster>();
        var panel = new GameObject("Recovery panel"); panel.transform.SetParent(root.transform, false);
        var background = panel.AddComponent<Image>(); background.color = new Color(.07f, .1f, .08f, .98f);
        var rect = panel.GetComponent<RectTransform>(); rect.anchorMin = new Vector2(.17f, .22f); rect.anchorMax = new Vector2(.83f, .78f);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        var label = new GameObject("Preserved save notice"); label.transform.SetParent(panel.transform, false);
        var text = label.AddComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 20; text.color = Color.white; text.alignment = TextAnchor.UpperLeft;
        text.text = "Fusion could not load or save this board. The preserved save has not been discarded.\n\n"
            + owner.Error + "\n\nRetry validates the preserved save again. Main menu leaves this attempt without saving.";
        var labelRect = label.GetComponent<RectTransform>(); labelRect.anchorMin = new Vector2(.05f, .3f); labelRect.anchorMax = new Vector2(.95f, .94f);
        labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
        retry = MakeButton(panel, owner.BranchChoiceRequired ? "Use current native save" : "Retry", new Vector2(.07f, .08f), new Vector2(.46f, .23f), () =>
        {
            try { FusionSaveLifecycle.Retry(owner); }
            catch (Exception error) { FusionSaveCoordinator.Hold(owner, error); Hide(); requested = true; }
        });
        var menu = MakeButton(panel, "Main menu without saving", new Vector2(.54f, .08f), new Vector2(.93f, .23f), () =>
        {
            try { FusionSaveLifecycle.MainMenu(owner); }
            catch (Exception error) { FusionSaveCoordinator.Hold(owner, error); }
        });
        var navigation = retry.navigation; navigation.mode = Navigation.Mode.Explicit; navigation.selectOnRight = menu;
        navigation.selectOnLeft = menu; retry.navigation = navigation;
        navigation = menu.navigation; navigation.mode = Navigation.Mode.Explicit; navigation.selectOnLeft = retry;
        navigation.selectOnRight = retry; menu.navigation = navigation;
        if (EventSystem.current != null)
        { previousSelection = EventSystem.current.currentSelectedGameObject; EventSystem.current.SetSelectedGameObject(retry.gameObject); }
    }
    private static Button MakeButton(GameObject parent, string caption, Vector2 min, Vector2 max, Action action)
    {
        var buttonObject = new GameObject(caption); buttonObject.transform.SetParent(parent.transform, false);
        var image = buttonObject.AddComponent<Image>(); image.color = new Color(.24f, .4f, .24f, 1);
        var button = buttonObject.AddComponent<Button>(); button.targetGraphic = image;
        var rect = buttonObject.GetComponent<RectTransform>(); rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
        var label = new GameObject("Label"); label.transform.SetParent(buttonObject.transform, false);
        var text = label.AddComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 17; text.text = caption; text.alignment = TextAnchor.MiddleCenter; text.color = Color.white; text.raycastTarget = false;
        var textRect = label.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        var listener = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction>(action)
            ?? throw new InvalidOperationException("Fusion recovery UI button delegate could not be registered.");
        listeners.Add(listener); button.onClick.AddListener(listener); AddCancel(buttonObject); return button;
    }
    private static void AddCancel(GameObject target)
    {
        var trigger = target.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.Cancel, callback = new EventTrigger.TriggerEvent() };
        var cancel = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction<BaseEventData>>(new Action<BaseEventData>(_ => Hide()))
            ?? throw new InvalidOperationException("Fusion recovery cancel delegate could not be registered.");
        cancelListeners.Add(cancel); entry.callback.AddListener(cancel); trigger.triggers.Add(entry);
    }
}

internal static class FusionSaveLifecycle
{
    private sealed record Exit(FusionSaveCoordinator.OwnerState Owner, IntPtr Manager, IntPtr Source, IntPtr Target, IntPtr Activity, long Generation)
    { internal bool Unloading; }
    private sealed record Request(TreeStateManager Manager, TreeState Target);
    internal sealed record LoadAuthorization(FusionSaveCoordinator.OwnerState Owner, IntPtr Service, long OwnerGeneration,
        IntPtr Activity, IntPtr Manager, IntPtr Target, long Sequence, string Binding, int Thread, long Attempt)
    { internal bool Consumed; }
    private sealed record EntryTicket(GameplayActivity Activity, FusionSaveCoordinator.OwnerState Owner,
        long Sequence, string Binding, int Thread, long Attempt);
    internal sealed class EndLevelAuthorization
    {
        internal readonly GameplayActivity Activity;
        internal readonly FusionSaveCoordinator.OwnerState Owner;
        internal readonly long Sequence;
        internal readonly string Binding;
        internal readonly int Thread;
        internal int Remaining = 2;
        internal EndLevelAuthorization(GameplayActivity activity, FusionSaveCoordinator.OwnerState owner, long sequence, string binding)
        { Activity = activity; Owner = owner; Sequence = sequence; Binding = binding; Thread = Environment.CurrentManagedThreadId; }
    }
    private sealed record QuitPermit(FusionSaveCoordinator.OwnerState Owner, IntPtr Board, long BoardGeneration,
        string Package, int Frame, long Deadline);
    private static Exit? recoveryExit;
    private static Request? retryRequest;
    private static QuitPermit? quit;
    private static bool quitRunning;
    private static long attemptCounter;
    private static readonly Dictionary<IntPtr, long> sequences = new();
    private static readonly Dictionary<IntPtr, EntryTicket> entries = new();
    [ThreadStatic] private static EntryTicket? activating;
    [ThreadStatic] private static LoadAuthorization? loading;
    [ThreadStatic] private static EndLevelAuthorization? ending;
    private static Il2CppSystem.Func<bool>? wantsToQuit;
    [ThreadStatic] internal static GameplayActivity? CoordinatedSave;
    internal sealed class SurvivalAuthorization
    {
        internal readonly FusionSaveCoordinator.OwnerState Owner;
        internal readonly Board Board;
        internal readonly long OwnerGeneration, BoardGeneration;
        internal readonly int Thread, Depth, Counter;
        internal readonly SurvivalAuthorization? Parent;
        internal string? Package, Binding;
        internal bool Consumed;
        internal SurvivalAuthorization(FusionSaveCoordinator.OwnerState owner, Board board, int depth, SurvivalAuthorization? parent)
        { Owner = owner; Board = board; OwnerGeneration = owner.Generation; BoardGeneration = FusionProjectileRecords.Generation(board);
            Thread = Environment.CurrentManagedThreadId; Depth = depth; Counter = 1; Parent = parent; }
    }
    [ThreadStatic] private static SurvivalAuthorization? duplicateSurvival;
    [ThreadStatic] private static int survivalDepth;
    private static GameplayActivity[] Activities(States states, TreeStateManager manager)
    {
        var result = new List<GameplayActivity>();
        for (int i = 0; i < states.Count; ++i)
        {
            var state = states[i];
            if (state.Manager?.Pointer != manager.Pointer) throw new InvalidDataException("Fusion transition manager identity changed.");
            for (int j = 0; j < state.m_activities.Count; ++j)
            {
                var activity = state.m_activities[j].TryCast<GameplayActivity>();
                if (activity != null) result.Add(activity);
            }
        }
        return result.ToArray();
    }
    private static bool Contains(States states, IntPtr pointer)
    { for (int i = 0; i < states.Count; ++i) if (states[i].Pointer == pointer) return true; return false; }
    internal static void Initialize()
    {
        wantsToQuit = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(new Func<bool>(WantsToQuit));
        if (wantsToQuit == null) throw new InvalidOperationException("Fusion wantsToQuit delegate could not be registered.");
        Application.add_wantsToQuit(wantsToQuit);
    }
    internal static void Dispose()
    {
        if (wantsToQuit != null) Application.remove_wantsToQuit(wantsToQuit);
        wantsToQuit = null; recoveryExit = null; retryRequest = null; quit = null;
    }
    internal static bool StartSequence(TreeStateManager manager, States unload, States load)
    {
        if (!FusionSaveCoordinator.Ready) return true;
        InvalidateTransition(manager);
        long sequence = sequences[manager.Pointer];
        var loading = Activities(load, manager); var unloading = Activities(unload, manager);
        if (recoveryExit is { } exit && exit.Manager == manager.Pointer && loading.Length == 0
            && exit.Owner.Generation == exit.Generation && FusionSaveCoordinator.IsCurrent(exit.Owner)
            && (manager.Active?.Pointer ?? IntPtr.Zero) == exit.Source && Contains(load, exit.Target)
            && unloading.All(activity => activity.Pointer == exit.Activity))
        { exit.Unloading = true; return true; }
        recoveryExit = null;
        foreach (var activity in unloading)
            if (FusionSaveCoordinator.Blocks(activity.Board)) throw new InvalidOperationException("Fusion save failure blocks native gameplay teardown.");
        foreach (var activity in loading)
        {
            retryRequest = new(manager, activity.TreeState);
            FusionSaveCoordinator.Preflight(activity);
            var owner = FusionSaveCoordinator.OwnerFor(activity);
            entries[activity.Pointer] = new(activity, owner, sequence, FusionSaveCoordinator.SelectedBinding(owner),
                Environment.CurrentManagedThreadId, checked(++attemptCounter));
        }
        return true;
    }
    internal static bool MayTearDown(GameplayActivity activity)
    {
        if (!FusionSaveCoordinator.Ready) return true;
        if (recoveryExit is { Unloading: true } exit && exit.Activity == activity.Pointer
            && exit.Owner.Generation == exit.Generation && activity.TreeState.Manager?.Pointer == exit.Manager) return true;
        return !FusionSaveCoordinator.Blocks(activity.Board);
    }
    internal static void InvalidateTransition(TreeStateManager manager)
    {
        sequences.TryGetValue(manager.Pointer, out long previous); sequences[manager.Pointer] = checked(previous + 1);
        foreach (var key in entries.Where(pair => pair.Value.Activity.TreeState.Manager?.Pointer == manager.Pointer).Select(pair => pair.Key).ToArray()) entries.Remove(key);
    }
    internal static void TransitionFailed(TreeStateManager manager)
    { InvalidateTransition(manager); if (recoveryExit?.Manager == manager.Pointer) recoveryExit = null; }
    internal static void BeginActivation(GameplayActivity activity)
    {
        if (activating != null || !entries.Remove(activity.Pointer, out var ticket))
            throw new InvalidOperationException("Gameplay activation lacks its validated Fusion transition token.");
        ValidateTicket(ticket); activating = ticket;
    }
    internal static void EndActivation() { activating = null; }
    private static void ValidateTicket(EntryTicket ticket)
    {
        if (ticket.Thread != Environment.CurrentManagedThreadId || !FusionSaveCoordinator.IsCurrent(ticket.Owner)
            || ticket.Activity.TreeState.Manager == null
            || !sequences.TryGetValue(ticket.Activity.TreeState.Manager.Pointer, out long sequence) || sequence != ticket.Sequence
            || FusionSaveCoordinator.SelectedBinding(ticket.Owner) != ticket.Binding)
            throw new InvalidDataException("Fusion transition authorization became stale.");
    }
    internal static bool BeginNewGame(GameplayActivity activity, out LoadAuthorization? authorization)
    {
        authorization = null;
        if (!FusionSaveCoordinator.Ready) return true;
        if (recoveryExit is { Unloading: true } exit && exit.Activity == activity.Pointer) return false;
        if (loading != null) throw new InvalidOperationException("Nested Fusion NewGame is unsupported.");
        FusionSaveCoordinator.OwnerState owner; long sequence; string binding; long attempt;
        if (ending is { } end && end.Activity.Pointer == activity.Pointer)
        {
            if (end.Remaining <= 0 || end.Thread != Environment.CurrentManagedThreadId || !FusionSaveCoordinator.IsCurrent(end.Owner)
                || FusionSaveCoordinator.SelectedBinding(end.Owner) != end.Binding)
                throw new InvalidDataException("Fusion EndLevel load authorization became stale.");
            --end.Remaining; owner = end.Owner; sequence = end.Sequence; binding = end.Binding; attempt = checked(++attemptCounter);
        }
        else
        {
            var ticket = activating;
            if (ticket == null || ticket.Activity.Pointer != activity.Pointer)
                throw new InvalidOperationException("Direct NewGame lacks its exact native caller authorization.");
            ValidateTicket(ticket); activating = null;
            owner = ticket.Owner; sequence = ticket.Sequence; binding = ticket.Binding; attempt = ticket.Attempt;
        }
        authorization = new(owner, owner.Service.Pointer, owner.Generation, activity.Pointer,
            activity.TreeState.Manager.Pointer, activity.TreeState.Pointer, sequence, binding, Environment.CurrentManagedThreadId, attempt);
        loading = authorization; return true;
    }
    internal static void EndNewGame(LoadAuthorization? authorization, Exception? error)
    {
        if (authorization == null || !ReferenceEquals(loading, authorization)) return;
        try { if (error == null && authorization.Consumed && ending == null) FusionSaveCoordinator.CompleteAcknowledgment(authorization.Owner); }
        catch (Exception failure) { FusionSaveCoordinator.Hold(authorization.Owner, failure); }
        finally { loading = null; }
    }
    internal static void ConsumeLoadAuthorization(SerializationService service, FusionSaveCoordinator.OwnerState owner, string binding)
    {
        var token = loading;
        if (token == null || token.Consumed || token.Service != service.Pointer || !ReferenceEquals(token.Owner, owner)
            || token.OwnerGeneration != owner.Generation || token.Thread != Environment.CurrentManagedThreadId
            || token.Activity != service.App?.Pointer || token.Manager != service.App?.TreeState.Manager?.Pointer
            || token.Target != service.App?.TreeState.Pointer || token.Binding != binding || !FusionSaveCoordinator.IsCurrent(owner)
            || !sequences.TryGetValue(token.Manager, out long current) || current != token.Sequence)
            throw new InvalidOperationException("Native Deserialize lacks its exact one-use Fusion NewGame token.");
        token.Consumed = true;
    }
    internal static FusionSaveCoordinator.OwnerState RequireNewBoardAuthorization(GameplayActivity activity)
    {
        var token = loading;
        if (token == null || token.Activity != activity.Pointer || token.Thread != Environment.CurrentManagedThreadId
            || !FusionSaveCoordinator.IsCurrent(token.Owner) || token.OwnerGeneration != token.Owner.Generation
            || FusionSaveCoordinator.SelectedBinding(token.Owner) != token.Binding)
            throw new InvalidOperationException("Native board replacement lacks an authorized Fusion NewGame scope.");
        return token.Owner;
    }
    internal static bool BeginEndLevel(GameplayActivity activity, out EndLevelAuthorization? authorization)
    {
        authorization = null;
        if (!FusionSaveCoordinator.Ready) return true;
        if (recoveryExit is { Unloading: true } exit && exit.Activity == activity.Pointer) return false;
        if (ending != null || loading != null) throw new InvalidOperationException("Nested Fusion EndLevel is unsupported.");
        FusionSaveCoordinator.Preflight(activity); var owner = FusionSaveCoordinator.OwnerFor(activity);
        var manager = activity.TreeState.Manager; sequences.TryGetValue(manager.Pointer, out long sequence);
        authorization = new(activity, owner, sequence, FusionSaveCoordinator.SelectedBinding(owner)); ending = authorization; return true;
    }
    internal static void EndEndLevel(EndLevelAuthorization? authorization, Exception? error)
    {
        if (authorization == null || !ReferenceEquals(ending, authorization)) return;
        try { if (error == null && authorization.Remaining == 0) FusionSaveCoordinator.CompleteAcknowledgment(authorization.Owner); }
        catch (Exception failure) { FusionSaveCoordinator.Hold(authorization.Owner, failure); }
        finally { ending = null; }
    }
    internal static void MainMenu(FusionSaveCoordinator.OwnerState owner)
    {
        owner.AcknowledgmentPending = false;
        var activity = owner.Service.App;
        var manager = activity?.TreeState?.Manager ?? retryRequest?.Manager
            ?? throw new InvalidOperationException("Native menu manager is unavailable; the save remains held.");
        var menu = Object.FindObjectsOfType<MainMenuActivity>(true).FirstOrDefault(value => value.TreeState?.Manager?.Pointer == manager.Pointer)
            ?? throw new InvalidOperationException("Native main menu is unavailable; the save remains held.");
        if (manager.Active?.Pointer == menu.TreeState.Pointer) { FusionSaveRecoveryUi.Hide(); return; }
        recoveryExit = new(owner, manager.Pointer, manager.Active?.Pointer ?? IntPtr.Zero,
            menu.TreeState.Pointer, activity?.Pointer ?? IntPtr.Zero, owner.Generation);
        if (manager.Active != null) manager.Active.TransitionTo(menu.TreeState); else manager.TransitionTo(menu.TreeState);
    }
    internal static void Retry(FusionSaveCoordinator.OwnerState owner)
    {
        bool retryLiveSave = owner.Service.App?.Board != null && !owner.Quarantined;
        FusionSaveCoordinator.Retry(owner);
        if (owner.Held) return;
        if (retryLiveSave)
        {
            var activity = owner.Service.App ?? throw new InvalidOperationException("Fusion retry gameplay activity is unavailable.");
            if (!Save(activity)) FusionSaveRecoveryUi.Show(owner); else FusionSaveCoordinator.CompleteAcknowledgment(owner);
            return;
        }
        if (retryRequest is { } request && request.Target.Manager?.Pointer == request.Manager.Pointer)
            request.Manager.TransitionTo(request.Target);
    }
    internal static bool Save(GameplayActivity activity)
    {
        if (CoordinatedSave != null) throw new InvalidOperationException("Nested Fusion gameplay save refused.");
        CoordinatedSave = activity;
        try { activity.SaveGame(); return FusionSaveCoordinator.LastSaveCommitted(activity); }
        finally { CoordinatedSave = null; }
    }
    internal static bool SaveEntry(GameplayActivity activity)
    {
        if (!FusionSaveCoordinator.Ready) return true;
        if (duplicateSurvival is { Consumed: false } permit && CoordinatedSave == null && permit.Board.Pointer == activity.Board?.Pointer
            && permit.Thread == Environment.CurrentManagedThreadId && permit.Depth == survivalDepth && permit.Counter == activity.Board.mNextSurvivalStageCounter
            && permit.OwnerGeneration == permit.Owner.Generation && FusionSaveCoordinator.IsCurrent(permit.Owner)
            && permit.BoardGeneration == FusionProjectileRecords.Generation(activity.Board) && permit.Package == permit.Owner.LastPackage
            && permit.Binding == FusionSaveCoordinator.SelectedBinding(permit.Owner))
        { permit.Consumed = true; return false; }
        if (recoveryExit is { Unloading: true } exit && exit.Activity == activity.Pointer) return false;
        return !FusionSaveCoordinator.Blocks(activity.Board);
    }
    internal static bool Survival(Board board, out SurvivalAuthorization? authorization)
    {
        authorization = null;
        if (!FusionSaveCoordinator.Ready) return true;
        if (FusionSaveCoordinator.Blocks(board)) return false;
        int counter = board.mNextSurvivalStageCounter;
        if (counter <= 0) return true;
        int effective = board.mApp.IsScaryPotterLevel() && board.mApp.CrazyDaveState != 0 ? counter : counter - 1;
        if (effective != 1 || !board.mApp.IsSurvivalMode()) return true;
        var owner = FusionSaveCoordinator.OwnerFor(board.mApp);
        authorization = new(owner, board, checked(++survivalDepth), duplicateSurvival);
        try
        {
            board.mNextSurvivalStageCounter = effective;
            if (!Save(board.mApp)) return false;
        }
        finally { board.mNextSurvivalStageCounter = counter; }
        authorization.Package = owner.LastPackage; authorization.Binding = FusionSaveCoordinator.SelectedBinding(owner);
        duplicateSurvival = authorization; return true;
    }
    internal static void EndSurvival(SurvivalAuthorization? authorization)
    {
        if (authorization == null || authorization.Thread != Environment.CurrentManagedThreadId) return;
        if (ReferenceEquals(duplicateSurvival, authorization)) duplicateSurvival = authorization.Parent;
        if (survivalDepth == authorization.Depth) --survivalDepth;
    }
    private static bool WantsToQuit()
    {
        if (!FusionSaveCoordinator.Ready) return true;
        var owner = FusionSaveCoordinator.ActiveOwner();
        if (owner?.Service.App?.Board is not { } board) return true;
        if (quitRunning || owner.Held || owner.Quarantined) { FusionSaveRecoveryUi.Show(owner); return false; }
        quitRunning = true;
        try
        {
            if (!Save(owner.Service.App) || owner.LastPackage == null) return false;
            if (owner.SessionEpoch != null) owner.Store.CloseSession(owner.SessionEpoch, owner.LastPackage);
            owner.SessionEpoch = null;
            quit = new(owner, board.Pointer, FusionProjectileRecords.Generation(board), owner.LastPackage,
                Time.frameCount, System.Diagnostics.Stopwatch.GetTimestamp() + 2 * System.Diagnostics.Stopwatch.Frequency);
            return true;
        }
        catch (Exception error) { FusionSaveCoordinator.Hold(owner, error); return false; }
        finally { quitRunning = false; }
    }
    internal static void QuitDispatch(bool permitted) { if (!permitted) quit = null; }
    internal static bool QuitCallback(GameplayActivity activity)
    {
        if (!FusionSaveCoordinator.Ready) return true;
        var permit = quit; quit = null;
        if (permit != null && activity.Board?.Pointer == permit.Board && permit.Owner.Service.App?.Pointer == activity.Pointer
            && permit.Owner.LastPackage == permit.Package && FusionProjectileRecords.Generation(activity.Board) == permit.BoardGeneration
            && Time.frameCount == permit.Frame && System.Diagnostics.Stopwatch.GetTimestamp() <= permit.Deadline) return false;
        if (activity.Board != null)
            FusionSaveCoordinator.FailActivity(activity, new IOException("Quit save was not confirmed; preserved snapshots remain available."));
        return false;
    }
    internal static bool QuitPending(Board? board) => quit != null && board?.Pointer == quit.Board;
    internal static void Update()
    {
        if (quit is { } permit && (Time.frameCount != permit.Frame || System.Diagnostics.Stopwatch.GetTimestamp() > permit.Deadline))
        { quit = null; FusionSaveCoordinator.Hold(permit.Owner, new IOException("Quit was not confirmed. Retry to resume from the preserved save.")); }
        if (recoveryExit is { Unloading: true } exit)
        {
            var menu = Object.FindObjectsOfType<MainMenuActivity>(true).FirstOrDefault(value => value.TreeState?.Pointer == exit.Target);
            if (menu?.TreeState.Manager?.Active?.Pointer == exit.Target && exit.Owner.Service.App?.Board == null)
            { exit.Owner.Held = exit.Owner.Quarantined = false; recoveryExit = null; FusionSaveRecoveryUi.Hide(); }
        }
    }
}

[HarmonyPatch(typeof(TreeStateManager), nameof(TreeStateManager.StartSequence))]
internal static class FusionSaveSequencePatch
{
    [HarmonyPriority(Priority.First + 3000)]
    private static bool Prefix(TreeStateManager __instance, States unloadStates, States loadStates)
    {
        try { return FusionSaveLifecycle.StartSequence(__instance, unloadStates, loadStates); }
        catch (Exception error) { FusionSaveCoordinator.FailTransition(__instance, loadStates, error); return false; }
    }
    private static void Finalizer(TreeStateManager __instance, Exception? __exception)
    { if (__exception != null) FusionSaveLifecycle.TransitionFailed(__instance); }
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.ActiveStarted))]
internal static class FusionSaveActivationPatch
{
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance, out bool __state)
    {
        __state = false; if (!FusionSaveCoordinator.Ready) return true;
        try { FusionSaveLifecycle.BeginActivation(__instance); __state = true; return true; }
        catch (Exception error) { FusionSaveCoordinator.FailActivity(__instance, error); return false; }
    }
    private static void Finalizer(bool __state) { if (__state) FusionSaveLifecycle.EndActivation(); }
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.NewGame))]
internal static class FusionSaveNewGamePatch
{
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance, out FusionSaveLifecycle.LoadAuthorization? __state)
    { try { return FusionSaveLifecycle.BeginNewGame(__instance, out __state); } catch (Exception error) { __state = null; FusionSaveCoordinator.FailActivity(__instance, error); return false; } }
    private static void Finalizer(FusionSaveLifecycle.LoadAuthorization? __state, Exception? __exception) => FusionSaveLifecycle.EndNewGame(__state, __exception);
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.EndLevel))]
internal static class FusionSaveEndLevelPatch
{
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance, out FusionSaveLifecycle.EndLevelAuthorization? __state)
    { try { return FusionSaveLifecycle.BeginEndLevel(__instance, out __state); } catch (Exception error) { __state = null; FusionSaveCoordinator.FailActivity(__instance, error); return false; } }
    private static void Finalizer(FusionSaveLifecycle.EndLevelAuthorization? __state, Exception? __exception) => FusionSaveLifecycle.EndEndLevel(__state, __exception);
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.MakeNewBoard))]
internal static class FusionSaveMakeBoardPatch
{
    [HarmonyPriority(Priority.First + 3000)]
    private static bool Prefix(GameplayActivity __instance, out FusionSaveCoordinator.OwnerState? __state)
    {
        __state = null; if (!FusionSaveCoordinator.Ready) return true;
        try { __state = FusionSaveLifecycle.RequireNewBoardAuthorization(__instance); return true; }
        catch (Exception error) { FusionSaveCoordinator.FailActivity(__instance, error); return false; }
    }
    private static void Postfix(GameplayActivity __instance, FusionSaveCoordinator.OwnerState? __state)
    {
        if (__state == null) return;
        try
        {
            if (!FusionSaveCoordinator.IsCurrent(__state) || __instance.Board == null) throw new InvalidDataException("Fusion new-board owner changed during initialization.");
            FusionProjectileRecords.BindNewBoard(__instance.Board, FusionSaveCoordinator.MapOwner(__state));
        }
        catch (Exception error) { FusionSaveCoordinator.Hold(__state, error, true); }
    }
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.KillBoard))]
internal static class FusionSaveKillPatch
{ [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance) => FusionSaveLifecycle.MayTearDown(__instance); }
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.DoBackToMain))]
internal static class FusionSaveMenuPatch
{ [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance) => FusionSaveLifecycle.MayTearDown(__instance); }
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.ActiveUpdate))]
internal static class FusionSaveActivityHoldPatch
{ [HarmonyPriority(Priority.First + 5000)] private static bool Prefix(GameplayActivity __instance) => !FusionSaveCoordinator.Blocks(__instance.Board); }
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.SaveGame))]
internal static class FusionSaveGameplaySavePatch
{ [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance) => FusionSaveLifecycle.SaveEntry(__instance); }
[HarmonyPatch(typeof(Board), "TryToSaveGame")]
internal static class FusionSavePrivateSavePatch
{
    [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(Board __instance)
    {
        if (!FusionSaveCoordinator.Ready || FusionSaveLifecycle.CoordinatedSave?.Board?.Pointer == __instance.Pointer) return true;
        FusionSaveCoordinator.FailActivity(__instance.mApp, new InvalidOperationException("Unscoped private board-save entry refused; its unknown caller cannot be aborted synchronously.")); return false;
    }
}
[HarmonyPatch(typeof(Board), nameof(Board.UpdateLevelEndSequence))]
internal static class FusionSaveSurvivalSavePatch
{
    [HarmonyPriority(Priority.Last - 100)] private static bool Prefix(Board __instance, out FusionSaveLifecycle.SurvivalAuthorization? __state)
    {
        __state = null;
        try { return FusionSaveLifecycle.Survival(__instance, out __state); }
        catch (Exception error) { FusionSaveCoordinator.FailActivity(__instance.mApp, error); return false; }
    }
    private static void Finalizer(FusionSaveLifecycle.SurvivalAuthorization? __state) => FusionSaveLifecycle.EndSurvival(__state);
}
[HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.OnApplicationQuit))]
internal static class FusionSaveQuitPatch
{ [HarmonyPriority(Priority.First + 3000)] private static bool Prefix(GameplayActivity __instance) => FusionSaveLifecycle.QuitCallback(__instance); }
[HarmonyPatch(typeof(Application), "Internal_ApplicationWantsToQuit")]
internal static class FusionSaveQuitDispatchPatch
{
    private static void Postfix(bool __result) => FusionSaveLifecycle.QuitDispatch(__result);
    private static void Finalizer(Exception? __exception) { if (__exception != null) FusionSaveLifecycle.QuitDispatch(false); }
}
