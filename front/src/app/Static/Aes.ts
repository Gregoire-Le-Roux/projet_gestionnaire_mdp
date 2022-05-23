import * as CryptoJS from 'crypto-js';
import { Mdp } from '../Types/Mdp';
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

        return TEXT_DECHIFFRER.replace(/[\u0000\x00\u001f\x1F\b]/g, "");
    }

    DechiffrerMdp(_liste: Mdp[]): Mdp[]
    {
        let listeRetour: Mdp[] = [];

        for (const element of _liste) 
        {
            listeRetour.push({
                Id: element.Id,
                Titre: this.Dechiffrer(element.Titre),
                Mdp: this.Dechiffrer(element.Mdp),
                Url: this.Dechiffrer(element.Url),
                Login: this.Dechiffrer(element.Login),
                DateExpiration: this.Dechiffrer(element.DateExpiration),
                IdCompteCreateur: element.IdCompteCreateur,
                Description: this.Dechiffrer(element.Description),
                EstVisible: false
            });
        }
        
        return listeRetour;
    }
}