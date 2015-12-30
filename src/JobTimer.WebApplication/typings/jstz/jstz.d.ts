interface JzTimeZone {
    name(): string;
}

interface JsTzStatic {
    determine(): JzTimeZone;
}

declare var jstz: JsTzStatic;

declare module "jstz" {
    export = jstz;
}
