Vagrant.configure(2) do |config|
    config.vm.define "dokku" do |mrd1|
      mrd1.vm.box = "ubuntu/bionic64"
      mrd1.vm.hostname = "dokku"
      mrd1.vm.network "public_network", bridge: 'ens33', ip: "192.168.82.156"
      mrd1.vm.provider "virtualbox" do |vb|
        vb.name = "ubuntu-1"
        vb.gui = false
        vb.memory = 1024
        vb.cpus = 2
      end
      mrd1.vm.provision "shell", path: "script2.sh"
    end
end
