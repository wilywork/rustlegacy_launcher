using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RustBuster.resources
{
    public class Translator
    {
        public static Dictionary<string, Dictionary<string, string>> SwithLanguage = new Dictionary<string, Dictionary<string, string>>();

        public static void Init() {

            SwithLanguage.Add("US", new Dictionary<string, string> {
                {"label_nickName", "Your Nick"},
                {"btn_playGame", "Play Game"},
                {"error_notFileStart", "Could not execute {0} file not existing or under  with another name."},
                {"label_status", "Check version..."},
                {"label_status_error", "Error verifying version."},
                {"start_download", "Starting downloads..."},
                {"downloading_file", "Downloading {0} ..."},
                {"finish_update", "Updated!"},
                {"download_error_antivirus", "There was an error, please try to disable your antivirus and try again."},
                {"textBlock_terms", "Terms of use"},
                {"text_terms","When you click play,\n"+
                    "You are automatically agreeing to these rules:\n\n"+
                    "You are aware that it is a private system designed to facilitate access to third-party servers and protects against fraud.\n\n"+
                    "To do this, RustBuster needs to have full access to the internal components of your system to have the ability to detect all hacks, with the mission of providing effective anti-cheat protection and not spy on you, just sharing your computer's serial with The RustBuster servers, for permanent punishment if you try to use some cheat program.\n\n"+
                    "You understand that you do not have the right to claim, file, or initiate a lawsuit against RustBuster, precisely because you performed the game in a free and conscious way, as explained in these rules, and in particular to determine that O User will be aware and automatically agree to the general terms and rules of this private organization when he / she clicks in play game."
                }

            });

            SwithLanguage.Add("BR", new Dictionary<string, string> {
                {"label_nickName", "Seu Nick"},
                {"btn_playGame", "Jogar"},
                {"error_notFileStart", "Não foi possível executar {0} arquivo não existente ou com outro nome."},
                {"label_status", "Verificando versão..."},
                {"label_status_error", "Erro ao verificar versão."},
                {"start_download", "Iniciando downloads..."},
                {"downloading_file", "Baixando {0} ..."},
                {"finish_update", "Atualizado!"},
                {"download_error_antivirus", "Ocorreu um erro, tente desativar seu antivirus e tente novamente."},
                {"textBlock_terms", "Termos de uso"},
                {"text_terms","Ao clicar em jogar,\n"+
                    "você estará automaticamente concordando com estas regras:\n\n"+
                    "Você está ciente de que este é um sistema privado desenvolvido para facilitar o acesso aos servidores de terceiros e protege-los de trapaças.\n\n"+
                    "Para isto, RustBuster precisa ter acesso completo aos componentes internos do seu sistema para ter a capacidade de detectar todos os hacks, com a missão de fornecer proteção anti-cheat eficaz e não para espionar você, tendo apenas o compartilhamento do serial do seu computador com os servidores RustBuster, para punição permanente caso tente utilizar algum programa de trapaça.\n\n"+
                    "O usuário/ utilizador entende e compreende que não possui nenhum direito perante a organização privada, seja para reclamar, exigir ou judicialmente iniciar processos contra a mesma, justamente por ter realizado o ato de ter utilizado / desfrutado do jogo de forma livre e consciente, como explicado nestas regras e, ressaltando, determinam que o usuário/ utilizador estará ciente e concordando automaticamente com os termos e regras gerais desta organização privada quando o mesmo clicar em jogar."
                }
            });

            SwithLanguage.Add("RU", new Dictionary<string, string> {
                {"label_nickName", "твой ник"},
                {"btn_playGame", "играть"},
                {"error_notFileStart", "Не удалось выполнить файл {0} не существует или под другим именем."},
                {"label_status", "Проверить версию..."},
                {"label_status_error", "Ошибка при проверке версии"},
                {"start_download", "Запуск загрузки..."},
                {"downloading_file", "Загрузка {0} ..."},
                {"finish_update", "обновленный!"},
                {"download_error_antivirus", "Был ошибка, пожалуйста, попробуйте отключить антивирус и повторите попытку."},
                {"textBlock_terms", "Условия использования"},
                {"text_terms","При нажатии кнопки воспроизведения,\n"+
                    "Вы автоматически соглашаетесь с этими правилами:\n\n"+
                    "Вы знаете, что это частная система, предназначенная для облегчения доступа к сторонним серверам и защищает от мошенничества.\n\n"+
                    "Для этого, RustBuster должен иметь полный доступ к внутренним компонентам вашей системы, чтобы иметь возможность обнаруживать все хаки, с миссией обеспечения эффективного античит защиты, а не шпионить за вами, просто делюсь серийный вашего компьютера с RustBuster серверы, на постоянное место наказания, если вы пытаетесь использовать какой-то обмануть программу.\n\n"+
                    "Вы понимаете, что вы не имеете права требовать, файл, или инициировать судебный процесс против RustBuster, именно потому, что вы выполнили игру в свободном и сознательным способом, как объяснено в этих правилах, и, в частности, чтобы определить, что O пользователь будет знать и автоматически соглашаетесь с общими условиями и правилами этой частной организации, когда он / она нажимает на играть в игру."
                }
            });

            SwithLanguage.Add("HU", new Dictionary<string, string> {
                {"label_nickName", "твой ник"},
                {"btn_playGame", "Játék Indítása"},
                {"error_notFileStart", "{0} nevű file nime található"},
                {"label_status", "Verzió Ellenőrzése..."},
                {"label_status_error", "Hiba a verzió ellenőrzése során"},
                {"start_download", "Letöltés Indítása..."},
                {"downloading_file", "{0} Letöltése..."},
                {"finish_update", "Frissítés kész!"},
                {"download_error_antivirus", "Egy ismeretlen hiba történt. Próbáld meg újra."},
                {"textBlock_terms", "Felhasználási Feltételek"},
                {"text_terms","Ha rákattintasz a játék gombra,\n"+
                    "automatikusan elfogadod ezeket a feltételeket:\n\n"+
                    "Ezen privát rendszer célja a moddolási lehetőségek megadása, illetve csalások ellen nyújtott védelem.\n\n"+
                    "Ehhez RustBusternek admin joggal kell rendelkeznie, mivel így képes megtalálni a csalásokat egyszerűbben, illetve bármilyen moddolt fileokat betud tölteni probléma nélkül. A rendszer csak adatokat közöl a szerverrel, bármilyen más kémkedésre utaló dolgot nem.\n\n"+
                    "A játék gombra elfogadtad a szabályzatot és az ezzel járó következményeket jogilag. Jó játékot!"
                }
            });

             SwithLanguage.Add("ES", new Dictionary<string, string> {
                {"label_nickName", "Su Nick"},
                {"btn_playGame", "Jugar"},
                {"error_notFileStart", "No se puede ejecutar {0} ,el archivo no existe o esta bajo otro nombre."},
                {"label_status", "Comprobando Version..."},
                {"label_status_error", "Error al comprobar la versión"},
                {"start_download", "Comenzando descarga..."},
                {"downloading_file", "Descargando {0} ..."},
                {"finish_update", "Actualizado!"},
                {"download_error_antivirus", "Se ha producido un error. Intenta desactivar tu antivirus y vuelva a intentarlo.."},
                {"textBlock_terms", "Terminos de uso"},
                {"text_terms","Al hacer clic en reproducir,\n"+
                    "Usted está aceptando automáticamente estas reglas:\n\n"+
                    "Usted es consciente de que se trata de un sistema privado diseñado para facilitar el acceso a servidores de terceros y protege contra el fraude.\n\n"+
                    "Para hacer esto, RustBuster necesita tener acceso completo a los componentes internos de su sistema para tener la capacidad de detectar todos los hacks, con la misión de proporcionar una protección anti-cheat efectiva y no espiar a usted, simplemente compartiendo la serie de su computadora con The RustBuster Servidores, para el castigo permanente si intenta utilizar algún programa de trucos.\n\n"+
                    "Usted entiende que usted no tiene el derecho de reclamar, archivar o iniciar una demanda contra RustBuster, precisamente porque usted realizó el juego de una manera libre y consciente, como se explica en estas reglas, y en particular para determinar que O Usuario será Consciente y automáticamente de acuerdo con los términos generales y las reglas de esta organización privada cuando hace clic en el juego."
                }
            });

        }

        public static string UpdateLanguage(string culture, string atribute) {
            if (SwithLanguage.ContainsKey(culture)) {
                if (SwithLanguage[culture].ContainsKey(atribute))
                {
                    return SwithLanguage[culture][atribute];
                }
                else if (SwithLanguage["US"].ContainsKey(atribute))
                {
                    return SwithLanguage["US"][atribute];
                }
                else {
                    return "Not Translator";
                }
            } else {
                if (SwithLanguage["US"].ContainsKey(atribute))
                {
                    return SwithLanguage["US"][atribute];
                }
                else
                {
                    return "Not Translator";
                }
            }
        }

        public static bool CultureList(string culture)
        {
            if (SwithLanguage.ContainsKey(culture))
            {
                return true;
            }
            else {
                return false;
            }
        }
    }

}
