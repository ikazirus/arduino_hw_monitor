class DataConfig:
    selected_port = "COM1"
    baud_rate = "9600"
    interval = 1

    def __init__(self, selected_port, baud_rate, interval):
        self.selected_port = selected_port
        self.baud_rate = baud_rate
        self.interval = interval


class PerformanceData:
    cpu_perc = 0.0
    gpu_perc = 0.0
    cpu_temp = 0.0
    gpu_temp = 0.0
    up_speed = 0.0
    dw_speed = 0.0

    def refresh(self, cpu_perc, ram_perc, up_speed, dw_speed):
        self.cpu_perc = cpu_perc
        self.ram_perc = ram_perc
        self.up_speed = up_speed
        self.dw_speed = dw_speed

        return f"C{self.cpu_perc}"
