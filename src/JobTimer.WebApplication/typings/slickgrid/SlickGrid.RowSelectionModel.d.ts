/// <reference path="slickgrid.d.ts" />

declare module Slick
{
	export class RowSelectionModel{
		onSelectedRangesChanged: Slick.Event<any>;
	}	
}