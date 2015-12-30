///<reference path="../jquery/jquery.d.ts" />

interface ISliderSlideEvent {
    value: number;
}

interface SliderOptions {
    tooltip?: string;
    value?: number;
    step?: number;
    min?: number;
    max?: number;
    ticks?: Array<number>;
    ticks_positions?: Array<number>;
    ticks_labels?: Array<string>;
    ticks_snaps_bounds?: number;
    formatter(value: number): string;
}

interface JQuery {
    slider(option?: SliderOptions): JQuery;
}