///<reference path="../jquery/jquery.d.ts" />
interface JScrollUpScrollImgOptions {
    active?: boolean;
    type?: string;
    src?: string;
}
interface JScrollUpOptions {
    scrollName?: string,
    scrollText?: string,
    topDistance?: string,
    topSpeed?: number,
    animation?: string,
    animationInSpeed?: number,
    animationOutSpeed?: number,
    scrollImg?: JScrollUpScrollImgOptions,
    activeOverlay?: boolean, // Set CSS color to display scrollUp active point, e.g '#00FFFF'
}

interface JQueryStatic {    
    scrollUp(option?: JScrollUpOptions);    
}