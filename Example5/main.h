//---------------------------------------------------------------------------

#ifndef mainH
#define mainH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
#include <ScktComp.hpp>
#include <ExtCtrls.hpp>
#include <Graphics.hpp>
#include <Mask.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
        TClientSocket *ClientSocket1;
        TMemo *Memo1;
        TEdit *IP;
        TEdit *Port;
        TBitBtn *Connect;
        TBitBtn *Disconnect;
        TLabel *Label1;
        TBitBtn *Clear;
        TGroupBox *GroupBox2;
        TBitBtn *Send;
        TComboBox *ComboBox0;
        TComboBox *ComboBox1;
        TComboBox *ComboBox2;
        TComboBox *ComboBox3;
        TComboBox *ComboBox4;
        TComboBox *ComboBox5;
        TComboBox *ComboBox6;
        TComboBox *ComboBox7;
        TComboBox *ComboBox8;
        TComboBox *ComboBox9;
        TComboBox *ComboBox10;
        TComboBox *ComboBox11;
        TLabel *Label2;
        TLabel *Label3;
        TLabel *Label4;
        void __fastcall ConnectClick(TObject *Sender);
        void __fastcall ClientSocket1Connect(TObject *Sender,
          TCustomWinSocket *Socket);
        void __fastcall DisconnectClick(TObject *Sender);
        void __fastcall ClientSocket1Error(TObject *Sender,
          TCustomWinSocket *Socket, TErrorEvent ErrorEvent,
          int &ErrorCode);
        void __fastcall ClientSocket1Read(TObject *Sender,
          TCustomWinSocket *Socket);
        void __fastcall Label1Click(TObject *Sender);
        void __fastcall SendClick(TObject *Sender);
        void __fastcall ClearClick(TObject *Sender);
        void __fastcall ComboBox0Change(TObject *Sender);
private:	// User declarations
public:		// User declarations

        __fastcall TForm1(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
