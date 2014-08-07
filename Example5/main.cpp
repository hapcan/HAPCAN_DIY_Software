//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "main.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
        : TForm(Owner)
{
}

//---------------------------------------------------------------------------
void __fastcall TForm1::Label1Click(TObject *Sender)
{
    ShellExecute( 0, "open", "http://hapcan.com", NULL, NULL, SW_NORMAL );         
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ClearClick(TObject *Sender)
{
   Memo1->Clear();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ConnectClick(TObject *Sender)
{
   ClientSocket1->Address = IP->Text;
   ClientSocket1->Port = StrToInt(Port->Text);
   ClientSocket1->Open();
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ClientSocket1Connect(TObject *Sender,
      TCustomWinSocket *Socket)
{
   Connect->Enabled = false;
   Send->Enabled = true;
   Disconnect->Enabled = true;
   Memo1->Lines->Add("Connected");
}
//---------------------------------------------------------------------------
void __fastcall TForm1::DisconnectClick(TObject *Sender)
{
   ClientSocket1->Close();
   Connect->Enabled = true;
   Disconnect->Enabled = false;
   Send->Enabled = false;
   Memo1->Lines->Add("Disonnected");
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ClientSocket1Error(TObject *Sender,
      TCustomWinSocket *Socket, TErrorEvent ErrorEvent, int &ErrorCode)
{
   switch (ErrorCode)
   {
      case 10054:
         Memo1 -> Lines -> Add("Error: Connection is reset by remote side.");
         DisconnectClick(Sender);
         Abort();
      default:
         Memo1 -> Lines -> Add("Error: " + IntToStr(ErrorCode));
         Abort();
   }
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ClientSocket1Read(TObject *Sender,
      TCustomWinSocket *Socket)
{
   unsigned char Buffer[15];
   AnsiString Line="";
   do{
      if(ClientSocket1->Socket->ReceiveLength()>14)
      {
         ClientSocket1->Socket->ReceiveBuf(Buffer,15);
         for(int j=0; j<15; j++)
            Line += IntToHex(Buffer[j],2)+" ";
         Memo1->Lines->Add("RX <-  "+Line);
         Line="";
      }
   }while(ClientSocket1->Socket->ReceiveLength()>0);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::SendClick(TObject *Sender)
{
   unsigned char Buffer[15];
   AnsiString Line;
   Buffer[0] = 0xAA;
   Buffer[13] = StrToInt("0x"+Label3->Caption);
   Buffer[14] = 0xA5;
   for(int i=1; i<13; i++)
      Buffer[i] = dynamic_cast<TComboBox *>(FindComponent("ComboBox"+IntToStr(i-1)))->ItemIndex;
   for(int i=0; i<15; i++)
      Line += IntToHex(Buffer[i],2)+" ";

   ClientSocket1->Socket->SendBuf(Buffer,15);
   Memo1->Lines->Add("-----");
   Memo1->Lines->Add("TX ->  "+Line);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ComboBox0Change(TObject *Sender)
{
   byte checksum = 0;
   for(int i=0; i<12; i++)
      checksum += dynamic_cast<TComboBox *>(FindComponent("ComboBox"+IntToStr(i)))->ItemIndex;
   Label3->Caption = IntToHex(checksum,2);
}
//---------------------------------------------------------------------------


