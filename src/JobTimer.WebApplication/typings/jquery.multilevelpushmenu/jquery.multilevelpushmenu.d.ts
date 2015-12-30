///<reference path="../jquery/jquery.d.ts" />

interface JMultiLevelPushMenuOptions {
    collapsed?: boolean,
    mode?: string,
    fullCollapse?: boolean,
    menuHeight?: number,
    preventItemClick?: boolean,
    direction?: string,
    onItemClick?();
}

interface JQuery {
    multilevelpushmenu(option?: JMultiLevelPushMenuOptions): JQuery;
    multilevelpushmenu(method: string): JQuery;
}