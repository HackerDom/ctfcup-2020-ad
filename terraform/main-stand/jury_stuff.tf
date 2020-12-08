

resource "yandex_compute_instance" "cs-main" {
  lifecycle {
    prevent_destroy = true
  }
  name = "cs-main"
  hostname = "cs-main"

  platform_id = "standard-v2"

  resources {
    cores  = 16
    memory = 32
    core_fraction = 100
  }

  scheduling_policy {
    preemptible = false
  }

  boot_disk {
    initialize_params {
      image_id = local.ubuntu-with-docker-id
      size = 100
      type = "network-ssd"
    }
  }

  network_interface {
    subnet_id = var.subnet_id
    ip_address = cidrhost(local.jury_subnet, 10)
    nat       = false
  }

  metadata = {
    ssh-keys = "ubuntu:${local.jury_ssh_key}"
  }
}

resource "yandex_compute_instance" "cs-worker" {
  lifecycle {
    prevent_destroy = true
  }
  name = "cs-worker"
  hostname = "cs-worker"

  resources {
    cores  = 16
    memory = 32
    core_fraction = 100
  }

  scheduling_policy {
    preemptible = false
  }

  boot_disk {
    initialize_params {
      image_id = local.ubuntu-with-docker-id
      size = 100
      type = "network-ssd"
    }
  }

  network_interface {
    subnet_id = var.subnet_id
    ip_address = cidrhost(local.jury_subnet, 11)
    nat       = false
  }

  metadata = {
    ssh-keys = "ubuntu:${local.jury_ssh_key}"
  }
}


resource "yandex_compute_instance" "proxy" {
  lifecycle {
    prevent_destroy = true
  }
  count = 2
  name = "proxy-${count.index}"
  hostname = "proxy-${count.index}"

  platform_id = "standard-v2"

  resources {
    cores  = 16
    memory = 32
    core_fraction = 100
  }

  scheduling_policy {
    preemptible = false
  }

  boot_disk {
    initialize_params {
      image_id = local.ubuntu-with-docker-id
      size = 100
      type = "network-ssd"
    }
  }

  network_interface {
    subnet_id = var.subnet_id
    ip_address = cidrhost(local.jury_subnet, 20 + count.index)
    nat       = false
  }

  metadata = {
    ssh-keys = "ubuntu:${local.jury_ssh_key}"
  }
}



