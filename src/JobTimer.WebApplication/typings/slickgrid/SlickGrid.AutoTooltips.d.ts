declare module Slick
{
	interface IAutoTooltipsOptions
	{
		enableForHeaderCells: boolean;
	}
	export class AutoTooltips{
		constructor(options?: IAutoTooltipsOptions);
	}	
}