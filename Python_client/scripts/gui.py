import os
from PIL import Image
import customtkinter

class MainWindow:
    port_list=[]
    port_names_list=[]
    selected_port = None
    
    customtkinter.set_appearance_mode("dark")
    customtkinter.set_default_color_theme("./scripts/res/themes/green.json")
    customtkinter.set_widget_scaling(1.2)
            
    def __init__(self, portlist):
      self.port_list= portlist
      for port in self.port_list:
          self.port_names_list.append(port.name)
    
    def open_window(self):       

        root = customtkinter.CTk()
        root.geometry("500x350")
        root.resizable(False, False)
        root.title("PerfMon - by Kaz")
        path = os.path.realpath("./scripts/res/img/icon.bmp")

        root.iconbitmap(path)

        frame = customtkinter.CTkFrame(master=root)
        frame.pack(pady=5, padx=5, fill="both", expand=True)

        logo = customtkinter.CTkImage(Image.open(os.path.realpath("./scripts/res/img/icon.png")),size=(40,26))

        logoLabel = customtkinter.CTkLabel(
            master=frame, text="", image=logo)
        logoLabel.pack(pady=4,padx=4)

        label = customtkinter.CTkLabel(
            master=frame, text="Login system", text_color="green",font=("Roboto",24))
        label.pack(pady=10,padx=10)

        entry1 = customtkinter.CTkEntry(master=frame, placeholder_text="Username")
        entry1.pack(pady=4, padx=4)

        entry2 = customtkinter.CTkEntry(
            master=frame, placeholder_text="Password", show="*")
        entry2.pack(pady=4, padx=4)

        button = customtkinter.CTkButton(master=frame, text="Login", command=None)
        button.pack(pady=4, padx=4)

        checkbox = customtkinter.CTkCheckBox(master=frame, text="Remember me")
        checkbox.pack(pady=4, padx=4)
        
        def select_com(val):
            print(val)
            
        dropbox = customtkinter.CTkComboBox(master=frame,values=self.port_names_list,command=select_com)
        dropbox.pack(pady=4, padx=4)
      
        root.mainloop()
        

