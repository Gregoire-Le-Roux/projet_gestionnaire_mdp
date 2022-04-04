import * as CryptoJS from 'crypto-js';
import { VariableStatic } from './VariableStatic';

export class Aes
{
    private cleSecrete: CryptoJS.lib.WordArray;
    private iv: CryptoJS.lib.WordArray;

    constructor(_cleSecrete: string)
    {
        this.cleSecrete = CryptoJS.enc.Utf8.parse(_cleSecrete.substring(0, 32));
        this.iv = CryptoJS.enc.Utf8.parse(_cleSecrete.substring(0, 16));
    }

    Chiffrer(_text: string): string
    {
        const TEXT_CHIFFRER: string = CryptoJS.AES.encrypt(_text, this.cleSecrete, { iv: this.iv }).toString();

        return TEXT_CHIFFRER;
    }

    Dechiffrer(_text: string): string
    {
        const TEXT_DECHIFFRER = CryptoJS.AES.decrypt(_text, this.cleSecrete, { iv: this.iv }).toString(CryptoJS.enc.Utf8);

        return TEXT_DECHIFFRER.replace(/[\u0000\x00]/g, "");
    }
}