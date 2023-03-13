namespace Sandbox2;

public partial class Test2 : Node
{
	public override void _Ready()
	{
		var slider = new UILabeledSlider(new UILabeledSliderOptions
		{
			InitialValue = 25,
			MaxValue = 100,
			Step = 0.1,
			Name = "Strength"
		});

		slider.ValueChanged += v =>
		{
			Logger.Log(v);
		};

		AddChild(slider);
	}
}
