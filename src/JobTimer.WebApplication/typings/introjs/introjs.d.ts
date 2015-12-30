interface IIntroJsHint {
    element?: Element;
    hint: string;
    hintPosition?: string;
    position?: string;
}
interface IIntroJsStep {
    intro: string;
    element?: Element;
    position?: string;
}
interface IIntroJsOptions {
    hints?: Array<IIntroJsHint>;
    steps?: Array<IIntroJsStep>;
}

interface IIntroJs {
    setOptions(options: IIntroJsOptions);
    onhintsadded(callback: Function);
    onhintclick(callback: Function);
    onhintclose(callback: Function);
    onexit(callback: Function);
    oncomplete(callback: Function);
    addHints();
    start();
    exit();
}

interface IIntroJsStatic {
    (): IIntroJs;
}

declare var introJs: IIntroJsStatic;

declare module "introJs" {
    export = introJs;
}
