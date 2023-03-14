namespace Sandbox2;

public partial class Test2 : Node
{
	public override void _Ready()
	{
		var slider = new UILabeledSlider(new LabeledSliderOptions
		{
			HSlider = new HSlider
			{
				MaxValue = 100,
				Step = 0.1,
			},
			Name = "Strength"
		});

		slider.ValueChanged += v =>
		{
			Logger.Log(v);
		};

		AddChild(slider);

		var optionButton = new UILabeledOptionButton(new LabeledOptionButtonOptions("SomeTest", "AnotherOption"));

		optionButton.ValueChanged += (item) =>
		{
			Logger.Log(item);
		};

		AddChild(optionButton);

		var lineEdit = new UILabeledLineEdit(new LabeledLineEditOptions
		{
			Name = "Username",
			LineEdit = new LineEdit
			{
				PlaceholderText = "Cheese",
				MaxLength = 5
			}
		});

		lineEdit.ValueChanged += text =>
		{
			Logger.Log(text);
		};

		AddChild(lineEdit);

		var checkBox = new UILabeledCheckbox(new LabeledCheckboxOptions
		{
			CheckBox = new CheckBox
			{
				Disabled = true
			}
		});
		checkBox.ValueChanged += v =>
		{
			Logger.Log(v);
		};
		AddChild(checkBox);

		var color = new UILabeledColorPickerButton(new LabeledColorPickerButtonOptions());
		color.ValueChanged += v => Logger.Log(v);
		AddChild(color);
	}
}
