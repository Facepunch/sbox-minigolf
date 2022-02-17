using System.Collections.Generic;

namespace Facepunch.Customization;

public partial class CustomizeConfig
{

	//public int CategoryIdAccumulator { get; set; }
	//public int PartIdAccumulator { get; set; }
	public List<CustomizationCategory> Categories { get; set; } = new();
	public List<CustomizationPart> Parts { get; set; } = new();

}

public class CustomizationCategory
{

	public int Id { get; set; }
	public string DisplayName { get; set; }
	public string IconPath { get; set; }
	public int DefaultPartId { get; set; }

}

public class CustomizationPart
{

	public int Id { get; set; }
	public int CategoryId { get; set; }
	public string DisplayName { get; set; }
	public string IconPath { get; set; }
	public string AssetPath { get; set; }

}
