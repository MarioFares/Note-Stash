using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace NoteStash.Models;

[AttributeUsage(AttributeTargets.Property)]
public class ExecutableActionAttribute : Attribute
{
	public readonly string Description;

	public readonly string Source;

	public readonly string InputGesture = string.Empty;

	public readonly string OptionsSource = string.Empty;

	public ExecutableActionAttribute(string description, string source)
	{
		Description = description;
		Source = source;
	}

	public ExecutableActionAttribute(string description, string source, string inputGesture = "", string options = "")
	{
		Description = description;
		Source = source;
		InputGesture = inputGesture;
		OptionsSource = options;
	}
}

/// <summary>
/// Model for an executable action, which either executes a command or toggles a boolean.
/// </summary>
public class ExecutableAction
{
	private readonly Func<bool>? _isEnabledGetter;

	private readonly Action<bool>? _isEnabledSetter;

	private readonly Func<ObservableCollection<string>?>? _argumentOptionsGetter;

	private readonly IRelayCommand? _command;

	/// <summary>
	/// Gets or sets the toggle state if there is one.
	/// </summary>
	private bool ToggleState
	{
		get => _isEnabledGetter();
		set =>_isEnabledSetter(value);
	}

	/// <summary>
	/// Gets the options to provide as an argument for the internal command.
	/// </summary>
	public ObservableCollection<string>? ArgumentOptions => _argumentOptionsGetter();

	/// <summary>
	/// Gets the description of the actions.
	/// </summary>
	public string Description { get; }

	/// <summary>
	/// Gets the source of the action, i.e. in what menu to find the action.
	/// </summary>
	public string Source { get; }

	/// <summary>
	/// Gets the input gesture associated with the command if there is one.
	/// </summary>
	public string InputGesture { get; }

	/// <summary>
	/// Returns true if the action to execute is toggling a boolean value.
	/// </summary>
	public bool Toggleable => _isEnabledGetter != null && _isEnabledSetter != null;

	/// <summary>
	/// Returns true if the internal command is not null and can execute. Returns false otherwise.
	/// </summary>
	public bool CommandCanExecute => _command != null
									 && _command.CanExecute(null!)
									 && ((!NeedsArgument) || (NeedsArgument && ArgumentOptions?.Count > 0));

	/// <summary>
	/// Returns true if the internal command is not null and requires a parameter to execute. Returns false otherwise.
	/// </summary>
	public bool NeedsArgument { get; }

	/// <summary>
	/// Initialize a toggleable action.
	/// </summary>
	public ExecutableAction(string description,
							string source,
							Func<bool> isEnabledGetter,
							Action<bool> isEnabledSetter,
							bool needsArgument = false)
	{
		Description = description;
		Source = source;
		_isEnabledGetter = isEnabledGetter;
		_isEnabledSetter = isEnabledSetter;
		NeedsArgument = needsArgument;
	}

	/// <summary>
	/// Initialize an action with a command.
	/// </summary>
	public ExecutableAction(string description,
							string source,
							string inputGesture,
							IRelayCommand command,
							Func<ObservableCollection<string>?>? optionsGetter = null)
	{
		Description = description;
		Source = source;
		InputGesture = inputGesture;
		_command = command;
		_argumentOptionsGetter = optionsGetter;
		NeedsArgument = optionsGetter != null && optionsGetter() != null;
	}

	/// <summary>
	/// Executes the internal command if <see cref="CommandCanExecute"/> is true
	/// or toggles the internal boolean <see cref="Toggleable"/> is true.
	/// </summary>
	public void Execute()
	{
		if (CommandCanExecute)
		{
			_command.Execute(null!);
		}
		else if (Toggleable)
		{
			ToggleState = !ToggleState;
		}
	}

	public void Execute(string arg)
	{
		if (CommandCanExecute) _command.Execute(arg);
	}
}
